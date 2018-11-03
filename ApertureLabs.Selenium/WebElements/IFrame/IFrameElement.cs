using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.IFrame
{
    /// <summary>
    /// Wrapper around the IWebElement for IFrames.
    /// </summary>
    public class IFrameElement : IWebElement
    {
        #region Fields

        private readonly IWebElement element;
        private readonly IWebDriver driver;

        private bool enteredIFrame = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <exception cref="InvalidCastException">
        /// Thrown if the element isn't an iframe.
        /// </exception>
        public IFrameElement(IWebElement element, IWebDriver driver)
        {
            if (!String.Equals(element.TagName, "iframe", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnexpectedTagNameException("Expected the tagname to be iframe.");
            }

            this.element = element;
            this.driver = driver;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string TagName => element.TagName;

        /// <inheritdoc/>
        public string Text => element.Text;

        /// <inheritdoc/>
        public bool Enabled => element.Enabled;

        /// <inheritdoc/>
        public bool Selected => element.Selected;

        /// <inheritdoc/>
        public Point Location => element.Location;

        /// <inheritdoc/>
        public Size Size => element.Size;

        /// <inheritdoc/>
        public bool Displayed => element.Displayed;

        #endregion

        #region Methods

        /// <summary>
        /// Enters the iframe only if not already entered.
        /// Equal to IWebDriver.SwitchTo().Frame(iframeElement).
        /// </summary>
        public void EnterIframe()
        {
            if (!enteredIFrame)
            {
                driver.SwitchTo().Frame(this);
                enteredIFrame = true;
            }
        }

        /// <summary>
        /// Exits the iframe only if previously entered.
        /// </summary>
        public void ExitIframe()
        {
            if (enteredIFrame)
            {
                driver.SwitchTo().ParentFrame();
                enteredIFrame = false;
            }
        }

        /// <inheritdoc/>
        public void Clear() => element.Clear();

        /// <inheritdoc/>
        public void SendKeys(string text) => element.SendKeys(text);

        /// <inheritdoc/>
        public void Submit() => element.Submit();

        /// <inheritdoc/>
        public void Click() => element.Click();

        /// <inheritdoc/>
        public string GetAttribute(string attributeName) => element.GetAttribute(attributeName);

        /// <inheritdoc/>
        public string GetProperty(string propertyName) => element.GetProperty(propertyName);

        /// <inheritdoc/>
        public string GetCssValue(string propertyName) => element.GetCssValue(propertyName);

        /// <inheritdoc/>
        public IWebElement FindElement(By by) => element.FindElement(by);

        /// <inheritdoc/>
        public ReadOnlyCollection<IWebElement> FindElements(By by) => element.FindElements(by);

        #endregion
    }
}
