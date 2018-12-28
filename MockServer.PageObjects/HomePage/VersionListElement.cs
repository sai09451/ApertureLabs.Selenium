using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace MockServer.PageObjects.HomePage
{
    public class VersionListElement
    {
        #region Fields

        private readonly IWebDriver driver;
        private readonly IWebElement element;

        #region Selectors

        private readonly By VersionStringSelector = By.CssSelector(".framework-version");
        private readonly By CollapsableSelector = By.CssSelector(".collapse");

        #endregion

        #endregion

        #region Constructor

        public VersionListElement(IWebElement element)
        {
            this.driver = element.GetDriver();
            this.element = element;
        }

        #endregion

        #region Properties

        public bool Expanded
        {
            get
            {
                return CollapsableElement.Classes().Contains("in");
            }
            set
            {
                if (Expanded != value)
                {
                    VersionStringElement.Click();
                    driver
                        .Wait(TimeSpan.FromSeconds(5))
                        .Until(d =>
                        {
                            return CollapsableElement.Classes().Contains("in") == value;
                        });
                }
            }
        }

        public string VersionString => VersionStringElement.Text;

        #region Elements

        private IWebElement VersionStringElement => element.FindElement(VersionStringSelector);
        private IWebElement CollapsableElement => element.FindElement(CollapsableSelector);

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
