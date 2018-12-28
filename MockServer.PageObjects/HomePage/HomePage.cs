using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace MockServer.PageObjects.HomePage
{
    public class HomePage : BasePage
    {
        #region Fields

        #region Selectors

        private readonly By FrameWorkSelector = By.CssSelector(".framework");

        #endregion

        #endregion

        #region Constructor

        public HomePage(IWebDriver driver, string url) : base(driver, url)
        { }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyList<FrameworkElement> FrameworkComponents => WrappedDriver
            .FindElements(FrameWorkSelector)
            .Select(e => new FrameworkElement(e))
            .ToList()
            .AsReadOnly();

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
