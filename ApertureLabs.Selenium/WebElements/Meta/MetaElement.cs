using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.Meta
{
    /// <summary>
    /// MetaElement.
    /// </summary>
    public class MetaElement : IWebElement
    {
        #region Fields

        private readonly IWebElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public MetaElement(IWebElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.TagName != "meta")
            {
                throw new InvalidElementStateException("Expected 'meta' as the tagname.");
            }

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

        /// <summary>
        /// Retrieves the value of the 'content' attribute.
        /// </summary>
        public string Content => element.GetAttribute("content");

        /// <summary>
        /// Returns the value of the 'name' attribute.
        /// </summary>
        public string Name => element.GetAttribute("name");

        #endregion

        #region Methods

        /// <inheritdoc/>
        public void Clear()
        {
            element.Clear();
        }

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

        #endregion
    }
}
