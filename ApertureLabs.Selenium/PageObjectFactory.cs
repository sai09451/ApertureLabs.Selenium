using ApertureLabs.Selenium.PageObjects;
using System;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// PageObjectFactory.
    /// </summary>
    public class PageObjectFactory : IPageObjectFactory
    {
        #region Methods

        /// <summary>
        /// Essentially just calls 'Load()' on the page object and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageObject"></param>
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
        public virtual T PreparePage<T>(T pageObject) where T : IPageObject
        {
            if (pageObject == null)
            {
                throw new ArgumentNullException(nameof(pageObject));
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
            {
                throw new ArgumentNullException(nameof(PageComponent));
            }

            return (T)pageComponent.Load();
        }

        #endregion
    }
}
