using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.Option
{
    /// <summary>
    /// OptionElement.
    /// </summary>
    public class OptionElement : IWebElement
    {
        #region Fields

        private readonly IWebElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public OptionElement(IWebElement element)
        {
            if (element.TagName != "option")
                throw new InvalidElementStateException("Expected tagname to be 'option'.");

            this.element = element;
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

        /// <inheritdoc/>
        public void Clear() => element.Clear();
        
        /// <inheritdoc/>
        public void Click()
        {
            element.Click();
        }

        /// <inheritdoc/>
        public IWebElement FindElement(By by)
        {
            return element.FindElement(by);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return element.FindElements(by);
        }

        /// <inheritdoc/>
        public string GetAttribute(string attributeName)
        {
            return element.GetAttribute(attributeName);
        }

        /// <inheritdoc/>
        public string GetCssValue(string propertyName)
        {
            return element.GetCssValue(propertyName);
        }

        /// <inheritdoc/>
        public string GetProperty(string propertyName)
        {
            return element.GetProperty(propertyName);
        }

        /// <inheritdoc/>
        public void SendKeys(string text)
        {
            element.SendKeys(text);
        }

        /// <inheritdoc/>
        public void Submit()
        {
            element.Submit();
        }

        /// <summary>
        /// Returns the value via attribute or if not present returns the text.
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            var value = GetAttribute("value");

            if (string.IsNullOrEmpty(value))
                value = Text;

            return value;
        }

        #endregion
    }
}
