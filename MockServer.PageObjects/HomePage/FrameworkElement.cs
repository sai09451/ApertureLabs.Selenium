using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace MockServer.PageObjects.HomePage
{
    public class FrameworkElement
    {
        #region Fields

        private readonly IWebElement element;
        private readonly IWebDriver driver;

        #region Selectors

        private readonly By NameSelector = By.CssSelector(".framework-name");

        #endregion

        #endregion

        #region Constructor

        public FrameworkElement(IWebElement element)
        {
            this.element = element;
            this.driver = element.GetDriver();
        }

        #endregion

        #region Properties

        public string Name => NameElement.Text;

        #region Elements

        private IWebElement NameElement => element.FindElement(NameSelector);

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
