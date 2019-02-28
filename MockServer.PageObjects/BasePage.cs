using ApertureLabs.Selenium.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;
using System;

namespace MockServer.PageObjects
{
    public class BasePage : StaticPageObject
    {
        #region Fields

        #region Selectors

        private readonly By NavbarHomeSelector = By.CssSelector("*[href='/']");
        private readonly IPageObjectFactory pageObjectFactory;

        #endregion

        #endregion

        #region Constructor

        public BasePage(IWebDriver driver,
            PageOptions pageOptions,
            IPageObjectFactory pageObjectFactory)
            : base(driver, new Uri(pageOptions.Url))
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));
            else if (pageOptions == null)
                throw new ArgumentNullException(nameof(pageOptions));
            else if (pageObjectFactory == null)
                throw new ArgumentNullException(nameof(pageObjectFactory));

            this.pageObjectFactory = pageObjectFactory;
            this.Uri = new Uri(pageOptions.Url);
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement NavbarHomeElement => WrappedDriver.FindElement(NavbarHomeSelector);

        #endregion

        #endregion

        #region Methods

        public HomePage GoToHomePage()
        {
            var homePage = pageObjectFactory.PreparePage<HomePage>();

            return homePage;
        }

        #endregion
    }
}
