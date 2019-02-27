﻿using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Used for preparing IPageObjects and IPageObjectFactories.
    /// </summary>
    public interface IPageObjectFactory
    {
        /// <summary>
        /// Essentially just calls 'Load()' on the page component and returns
        /// it. This is useful for initializing a component in one line.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="loadableComponent"></param>
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
        T PrepareComponent<T>(T loadableComponent) where T : ILoadableComponent;

        /// <summary>
        /// Creates an instance of the PageObject using the service provider to
        /// resolve the constructor arguments and then calls 'Load()' on it.
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
        T PreparePage<T>() where T : IPageObject;

        /// <summary>
        /// Gets the imported modules. Exists solely to verify the correct
        /// modules are being imported.
        /// </summary>
        /// <returns></returns>
        IList<IOrderedModule> GetImportedModules();
    }
}