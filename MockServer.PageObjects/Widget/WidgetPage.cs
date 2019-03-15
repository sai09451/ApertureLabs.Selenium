using ApertureLabs.Selenium.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;
using System;

namespace MockServer.PageObjects.Widget
{
    public class WidgetPage : ParameterPageObject, IBasePage
    {
        #region Fields

        private readonly IBasePage basePage;

        #endregion

        #region Constructor

        public WidgetPage(IBasePage basePage,
            IWebDriver driver,
            PageOptions pageOptions)
            : base(driver,
                  new Uri(pageOptions.Url),
                  new UriTemplate("{framework}/{version}/{widget}"))
        {
            this.basePage = basePage;
        }

        #endregion

        #region Methods

        public virtual HomePage GoToHomePage()
        {
            return basePage.GoToHomePage();
        }

        #endregion
    }
}
