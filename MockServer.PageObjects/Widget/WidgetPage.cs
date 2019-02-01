using ApertureLabs.Selenium;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace MockServer.PageObjects.Widget
{
    public class WidgetPage : BasePage
    {
        #region Constructor

        public WidgetPage(IWebDriver driver,
            PageOptions pageOptions,
            IPageObjectFactory pageObjectFactory)
            : base(driver, pageOptions, pageObjectFactory)
        { }

        #endregion
    }
}
