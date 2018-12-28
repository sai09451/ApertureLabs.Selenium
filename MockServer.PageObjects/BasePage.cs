using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;

namespace MockServer.PageObjects
{
    public class BasePage : PageObject
    {
        #region Fields

        #region Selectors

        private readonly By NavbarHomeSelector = By.CssSelector("*[href='/']");

        #endregion

        #endregion

        #region Constructor

        public BasePage(IWebDriver driver, string url) : base(driver)
        {
            Uri = new Uri(url);
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement NavbarHomeElement => WrappedDriver.FindElement(NavbarHomeSelector);

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
