using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium;
using ApertureLabs.Selenium.PageObjects;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace MockServer.PageObjects.Home
{
    public class HomePage : BasePage
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By FrameWorkSelector = By.CssSelector(".framework");
        private readonly By FrameworkNameSelector = By.CssSelector(".framework-name");
        private readonly By VersionsSelector = By.CssSelector(".framework-version");
        private readonly By WidgetsSelector = By.CssSelector(".framework-version-path a");

        #endregion

        #endregion

        #region Constructor

        public HomePage(IWebDriver driver,
            PageOptions pageOptions,
            IPageObjectFactory pageObjectFactory)
            : base(driver, pageOptions, pageObjectFactory)
        {
            this.pageObjectFactory = pageObjectFactory;
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyList<IWebElement> FrameworkElements => WrappedDriver.FindElements(FrameWorkSelector);

        #endregion

        #endregion

        #region Methods

        public WidgetPage GoToWidget(string frameworkName,
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
            if (!WrappedDriver.Url.StartsWith("http"))
                WrappedDriver.Navigate().GoToUrl(Uri.ToString());

            return base.Load();
        }

        #endregion
    }
}
