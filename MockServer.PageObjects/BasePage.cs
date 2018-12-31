using ApertureLabs.Selenium;
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
        private readonly IPageObjectFactory pageObjectFactory;

        #endregion

        #endregion

        #region Constructor

        public BasePage(IWebDriver driver,
            string url,
            IPageObjectFactory pageObjectFactory = null) : base(driver)
        {
            this.pageObjectFactory = pageObjectFactory ?? new PageObjectFactory();
            this.Uri = new Uri(url);
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement NavbarHomeElement => WrappedDriver.FindElement(NavbarHomeSelector);

        #endregion

        #endregion

        #region Methods

        public HomePage.HomePage GoToHomePage()
        {
            var homePage = new HomePage.HomePage(WrappedDriver,
                Uri.ToString(),
                pageObjectFactory);
            return pageObjectFactory.PreparePage(homePage);
        }

        #endregion
    }
}
