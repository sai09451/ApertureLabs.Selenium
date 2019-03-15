using ApertureLabs.Selenium.PageObjects;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockServer.PageObjects.Home
{
    public class HomePage : StaticPageObject, IBasePage
    {
        #region Fields

        #region Selectors

        private readonly By FrameWorkSelector = By.CssSelector(".framework");
        private readonly By FrameworkNameSelector = By.CssSelector(".framework-name");
        private readonly By VersionsSelector = By.CssSelector(".framework-version");
        private readonly By WidgetsSelector = By.CssSelector(".framework-version-path a");

        #endregion

        private readonly IBasePage basePage;
        private readonly IPageObjectFactory pageObjectFactory;

        #endregion

        #region Constructor

        public HomePage(IBasePage basePage,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver,
            PageOptions pageOptions)
            : base(driver, new Uri(pageOptions.Url))
        {
            this.basePage = basePage;
            this.pageObjectFactory = pageObjectFactory;
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyList<IWebElement> FrameworkElements => WrappedDriver
            .FindElements(FrameWorkSelector);

        public virtual HomePage GoToHomePage()
        {
            return basePage.GoToHomePage();
        }

        #endregion

        #endregion

        #region Methods

        public virtual WidgetPage GoToWidget(string frameworkName,
            string version,
            string widget)
        {
            // Find framework.
            var frameworkEl = FrameworkElements.FirstOrDefault(
                fc => String.Equals(
                    fc.FindElement(FrameworkNameSelector).Text,
                    frameworkName,
                    StringComparison.OrdinalIgnoreCase));

            if (frameworkEl == null)
                throw new Exception("Failed to find framework.");

            // Find version.
            var versionEl = frameworkEl.FindElements(VersionsSelector)
                .FirstOrDefault(ve => String.Equals(
                    ve.Text,
                    version,
                    StringComparison.Ordinal));

            if (versionEl == null)
                throw new Exception("Failed to find version.");

            // Find widget.
            var widgetContainerSelector = versionEl.GetAttribute("href");
            var widgetContainerEl = frameworkEl.FindElement(By.CssSelector(widgetContainerSelector));
            var widgetEl = widgetContainerEl.FindElements(WidgetsSelector)
                .FirstOrDefault(we => String.Equals(
                    we.Text,
                    widget,
                    StringComparison.OrdinalIgnoreCase));

            if (widgetEl == null)
                throw new Exception("Failed to find the widget.");

            widgetEl.Click();

            var widgetPage = pageObjectFactory.PreparePage<WidgetPage>();

            return widgetPage;
        }

        public override ILoadableComponent Load()
        {
            base.Load();
            basePage.Load();

            return this;
        }

        #endregion
    }
}
