using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.IFrame
{
    public class IFrameHelper : IWebElement
    {
        #region Fields

        private bool enteredIFrame = false;
        private IWebElement webElement;
        private readonly IWebDriver driver;

        #endregion

        #region Constructor

        public IFrameHelper(IWebElement element, IWebDriver driver)
        {
            if (String.Compare(element.TagName, "iframe", true) != 0)
            {
                throw new InvalidCastException("The elements tag name wasn't" +
                    " iframe.");
            }
        }

        #endregion

        #region Properties

        public string TagName => webElement.TagName;

        public string Text => webElement.Text;

        public bool Enabled => webElement.Enabled;

        public bool Selected => webElement.Selected;

        public Point Location => webElement.Location;

        public Size Size => webElement.Size;

        public bool Displayed => webElement.Displayed;

        #endregion

        #region Methods

        public void EnterIframe()
        {
            if (!enteredIFrame)
            {
                driver.SwitchTo().ParentFrame();
                enteredIFrame = true;
            }
        }

        public void ExitIframe()
        {
            if (enteredIFrame)
            {
                driver.SwitchTo().ParentFrame();
                enteredIFrame = false;
            }
        }

        public void Clear() => webElement.Clear();
        public void SendKeys(string text) => webElement.SendKeys(text);
        public void Submit() => webElement.Submit();
        public void Click() => webElement.Click();
        public string GetAttribute(string attributeName) => webElement.GetAttribute(attributeName);
        public string GetProperty(string propertyName) => webElement.GetProperty(propertyName);
        public string GetCssValue(string propertyName) => webElement.GetCssValue(propertyName);
        public IWebElement FindElement(By by) => webElement.FindElement(by);
        public ReadOnlyCollection<IWebElement> FindElements(By by) => webElement.FindElements(by);

        #endregion
    }
}
