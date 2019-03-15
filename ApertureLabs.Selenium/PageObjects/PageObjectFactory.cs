using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// PageObjectFactory.
    /// </summary>
    public class PageObjectFactory : IPageObjectFactory
    {
        #region Fields

        private readonly IContainer serviceProvider;
        private readonly IList<IOrderedModule> importedModules;

        #endregion

        #region Constructor

        private PageObjectFactory()
        {
            importedModules = new List<IOrderedModule>();
            LoadAssemblies();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectFactory"/> class.
        /// </summary>
        /// <param name="modules">The modules.</param>
        /// <param name="services">
        /// The services. Make sure a web driver has been registered as
        /// <c>IWebDriver</c> or else this will throw an Exception.
        /// </param>
        public PageObjectFactory(
            IEnumerable<IOrderedModule> modules,
            IServiceCollection services)
            : this()
        {
            if (modules == null)
                throw new ArgumentNullException(nameof(modules));
            else if (services == null)
                throw new ArgumentNullException(nameof(services));

            var driverRegistered = services
                .Any(s => s.ServiceType == typeof(IWebDriver));

            if (!driverRegistered)
            {
                throw new ArgumentException("The services collection must " +
                    "have the IWebDriver interface registered.");
            }

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IPageObjectFactory>(this);
            var orderedModules = modules.OrderBy(m => m.Order);

            foreach (var moduleType in orderedModules)
            {
                containerBuilder.RegisterModule(moduleType);
                importedModules.Add(moduleType);
            }

            containerBuilder.Populate(services);
            serviceProvider = containerBuilder.Build();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectFactory"/>
        /// class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="scanAssemblies">
        /// Scans all assemblies for page objects and registers them as
        /// singletons.
        /// </param>
        /// <param name="loadModules"></param>
        /// <param name="ignoredTypesAndModules">
        /// These types will not be loaded into the service provider. They
        /// can be types derived from IOrderedModule or IPageIObject.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if services doens't have IWebDriver registered.
        /// </exception>
        public PageObjectFactory(IServiceCollection services,
            bool scanAssemblies = true,
            bool loadModules = true,
            IEnumerable<Type> ignoredTypesAndModules = null)
            : this()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var driverRegistered = services
                .Any(s => s.ServiceType == typeof(IWebDriver));

            if (!driverRegistered)
            {
                throw new ArgumentException("The services collection must " +
                    "have the IWebDriver interface registered.");
            }

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IPageObjectFactory>(this);

            // Scan assemblies.
            if (scanAssemblies)
                ScanAssemblies(containerBuilder);

            // Load modules.
            if (loadModules)
                LoadModules(containerBuilder, ignoredTypesAndModules);


            // Then load the passed in services. This way all overrides will
            // remain.
            containerBuilder.Populate(services);
            serviceProvider = containerBuilder.Build();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectFactory"/>
        /// class. Uses reflection to create the service provider. All
        /// IPageObjects will be registered and then IOrderedModules will be
        /// registered.
        /// </summary>
        /// <param name="driver"></param>
        public PageObjectFactory(IWebDriver driver) : this()
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IPageObjectFactory>(this);
            containerBuilder.RegisterInstance<IWebDriver>(driver);
            ScanAssemblies(containerBuilder);
            LoadModules(containerBuilder);

            serviceProvider = containerBuilder.Build();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Essentially just calls 'Load()' on the page object (which is
        /// constructed with the service provider) and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <example>
        /// Before:
        /// // Instantiated but unintialized.
        /// var page = new YourPage(arg1, arg2);
        /// // Initialized.
        /// page.Load();
        /// 
        /// After:
        /// // Instantiate and initialize in one (albeit long) line.
        /// var page = pageObjectFactory.PreparePage(new YourPage(arg1, arg2));
        /// </example>
        public virtual T PreparePage<T>() where T : IPageObject
        {
            var pageObject = serviceProvider.Resolve<T>();

            if (pageObject == null)
            {
                throw new ArgumentException("No such page object registered " +
                    "with the service provider.");
            }

            return (T)pageObject.Load();
        }

        /// <summary>
        /// Essentially just calls 'Load()' on the page component and returns
        /// it. This is useful for initializing a component in one line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageComponent"></param>
        /// <returns></returns>
        /// <example>
        /// Before:
        /// // Uninitialized
        /// var component = new YourComponent(arg1, arg2);
        /// // Initialized
        /// component.Load();
        /// 
        /// After:
        /// // Instantiate and initialize in one (ableit long) line.
        /// var initializedcomponent = yourPageObjectFactory
        ///     .PrepareComponent(new YourComponent(arg1, arg2));
        /// </example>
        public virtual T PrepareComponent<T>(T pageComponent) where T : ILoadableComponent
        {
            if (pageComponent == null)
                throw new ArgumentNullException(nameof(pageComponent));

            return (T)pageComponent.Load();
        }

        /// <summary>
        /// Gets the imported modules. Exists solely to check if the correct
        /// modules are being imported.
        /// </summary>
        /// <returns></returns>
        public virtual IList<IOrderedModule> GetImportedModules()
        {
            return importedModules;
        }

        private void ScanAssemblies(ContainerBuilder containerBuilder,
            IEnumerable<Type> ignoredTypesAndModules = null)
        {
            ignoredTypesAndModules = ignoredTypesAndModules ?? new List<Type>();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Use reflection to load all types that inherit from IPageObject
            // and IPageComponent.
            containerBuilder.RegisterAssemblyTypes(loadedAssemblies)
                .Where(t =>
                {
                    var useType = (t.IsAssignableTo<IPageObject>())
                        && !t.IsAbstract
                        && t.IsClass
                        && t.IsVisible
                        && !ignoredTypesAndModules.Contains(t);

                    return useType;
                })
                .PublicOnly()
                .InstancePerLifetimeScope();
        }

        private void LoadModules(ContainerBuilder containerBuilder,
            IEnumerable<Type> ignoredModules = null)
        {
            ignoredModules = ignoredModules ?? new List<Type>();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var modules = loadedAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.IsPublic
                    && t.IsVisible
                    && t.IsAssignableTo<IOrderedModule>()
                    && t.GetConstructors().Any(
                        c => c.IsPublic && !c.GetParameters().Any())
                    && !ignoredModules.Contains(t))
                .Select(InstantiateModule)
                .OrderBy(om => om.Order)
                .ToList();

            foreach (var module in modules)
            {
                containerBuilder.RegisterModule(module);
                importedModules.Add(module);
            }
        }

        /// <summary>
        /// This is used as a workaround for assemblies not loading until one
        /// of their types are referenced.
        /// </summary>
        private void LoadAssemblies()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                LoadReferencedAssembly(assembly);
        }

        private IOrderedModule InstantiateModule(Type type)
        {
            var ctor = type.GetMatchingConstructor(Array.Empty<Type>());
            var instance = ctor.Invoke(Array.Empty<object>());

            if (instance is IOrderedModule orderedModule)
            {
                return orderedModule;
            }

            throw new InvalidCastException($"Invalid module: {type.Name}");
        }

        private void LoadReferencedAssembly(Assembly assembly)
        {
            foreach (AssemblyName name in assembly.GetReferencedAssemblies())
            {
                var isLoaded = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Any(a => a.FullName == name.FullName);

                if (!isLoaded)
                {
                    try
                    {
                        LoadReferencedAssembly(Assembly.Load(name));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        #endregion
    }
}
