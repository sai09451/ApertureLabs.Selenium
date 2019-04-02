using ApertureLabs.Selenium.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Default implementation of IPageObject. Used to represent web-pages
    /// which have urls that don't have any variables in them. Use the
    /// <see cref="ParameterPageObject" /> class for page objects whose urls
    /// do have parameters.
    /// </summary>
    public abstract class StaticPageObject : PageObject, IStaticPageObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticPageObject"/>
        /// class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="route">The route.</param>
        public StaticPageObject(IWebDriver driver, Uri route)
            : base(driver,
                  new Uri(route.GetLeftPart(UriPartial.Authority)),
                  new UriTemplate(route.GetComponents(
                      UriComponents.PathAndQuery,
                      UriFormat.Unescaped)))
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Tries to navigate to the IPageObjects Uri before executing
        /// <c>Load</c>. If the Uri is null and the UriMatcher contains groups
        /// this will throw an exception.
        /// </summary>
        /// <param name="firstNavigateToUrl">if set to <c>true</c> [first navigate to URL].</param>
        /// <returns></returns>
        public virtual ILoadableComponent Load(bool firstNavigateToUrl)
        {
            NavigateToUri();

            return Load();
        }

        /// <summary>
        /// Loads the component. Checks to see if the current url matches
        /// the Route and if not an exception is thrown. If the WrappedDriver
        /// is an <see cref="T:OpenQA.Selenium.Support.Events.EventFiringWebDriver" /> event listeners will be
        /// added to the <see cref="E:OpenQA.Selenium.Support.Events.EventFiringWebDriver.Navigated" /> event
        /// which will call <see cref="M:ApertureLabs.Selenium.PageObjects.PageObject.Dispose" /> on this instance.
        /// NOTE:
        /// If overriding don't forget to either call base.Load() or make sure
        /// the <see cref="P:ApertureLabs.Selenium.PageObjects.PageObject.Uri" /> and the <see cref="P:ApertureLabs.Selenium.PageObjects.PageObject.WindowHandle" /> are
        /// assigned to.
        /// </summary>
        /// <returns>
        /// A reference to this
        /// <see cref="T:OpenQA.Selenium.Support.UI.ILoadableComponent" />.
        /// </returns>
        public override ILoadableComponent Load()
        {
            // Navigate to the url if the current url is the default open url
            // or by some error it's null/empty.
            if (String.IsNullOrEmpty(WrappedDriver.Url)
                || WrappedDriver.Url == "data:,"
                || WrappedDriver.Url == "about:blank")
            {
                NavigateToUri();
            }

            return base.Load();
        }

        private void NavigateToUri()
        {
            var uri = Route.BindByName(
                BaseUri,
                new Dictionary<string, string>());

            var url = uri.ToString();
            var alreadyOnUrl = String.Equals(
                url,
                WrappedDriver.Url,
                StringComparison.OrdinalIgnoreCase);

            if (!alreadyOnUrl)
                WrappedDriver.Navigate().GoToUrl(uri);
        }

        #endregion
    }
}
