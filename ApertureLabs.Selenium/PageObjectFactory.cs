using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.PageObjects;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// PageObjectFactory.
    /// </summary>
    public class PageObjectFactory : IPageObjectFactory
    {
        #region Fields

        private readonly IContainer serviceProvider;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectFactory"/>
        /// class.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="scanAssemblies"></param>
        /// <exception cref="System.ArgumentException">
        /// Thrown if services doens't have IWebDriver registered.
        /// </exception>
        public PageObjectFactory(IServiceCollection services,
            bool scanAssemblies = true)
        {
            var driverRegistered = services
                .Any(s => s.ServiceType == typeof(IWebDriver));

            if (!driverRegistered)
            {
                throw new ArgumentException("The services collection must " +
                    "have the IWebDriver interface registered.");
            }

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            if (scanAssemblies)
                ScanAssemblies(containerBuilder);

            containerBuilder.RegisterInstance<IPageObjectFactory>(this);
            serviceProvider = containerBuilder.Build();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObjectFactory"/> class.
        /// Uses reflection to create the service provider.
        /// </summary>
        /// <param name="driver"></param>
        public PageObjectFactory(IWebDriver driver)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance<IPageObjectFactory>(this);
            containerBuilder.RegisterInstance<IWebDriver>(driver);
            ScanAssemblies(containerBuilder);

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
        public virtual T PrepareComponent<T>(T pageComponent) where T : IPageComponent
        {
            if (pageComponent == null)
                throw new ArgumentNullException(nameof(pageComponent));

            return (T)pageComponent.Load();
        }

        private void ScanAssemblies(ContainerBuilder containerBuilder)
        {
            // Use reflection to load all types that inherit from IPageObject
            // and IPageComponent.
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            containerBuilder.RegisterAssemblyTypes(loadedAssemblies)
                .Where(t => (t.IsAssignableTo<IPageObject>())
                    && !t.IsAbstract
                    && t.IsClass
                    && t.IsVisible)
                .PublicOnly()
                .SingleInstance();
        }

        #endregion
    }
}
