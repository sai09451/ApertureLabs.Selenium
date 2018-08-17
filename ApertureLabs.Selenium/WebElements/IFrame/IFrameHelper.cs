using ApertureLabs.Selenium.WebDriver;
using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using System;

namespace ApertureLabs.Selenium.WebElements.IFrame
{
    public class IFrameHelper : WebElementWrapper
    {
        #region Fields

        private bool enteredIFrame = false;

        #endregion

        #region Constructor

        public IFrameHelper(IWebElement element, WebDriverV2 driver)
            : base(element, driver)
        {
            if (string.Compare(WebElement.TagName, "iframe", true) != 0)
            {
                throw new InvalidCastException("The elements tag name wasn't" +
                    " iframe.");
            }
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void EnterIframe()
        {
            if (!enteredIFrame)
            {
                driver.WebDriver.SwitchTo().ParentFrame();
                enteredIFrame = true;
            }
        }

        public void ExitIframe()
        {
            if (enteredIFrame)
            {
                driver.WebDriver.SwitchTo().ParentFrame();
                enteredIFrame = false;
            }
        }

        #endregion
    }
}
