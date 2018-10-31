using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.Canvas
{
    /// <summary>
    /// CanvasElement.
    /// </summary>
    public class CanvasElement : IWebElement
    {
        #region Fields

        private readonly IWebElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public CanvasElement(IWebElement element)
        {
            this.element = element;

            if (!String.Equals(element.TagName,
                "canvas",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidElementStateException("Expected tagname to be 'canvas'.");
            }
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

        /// <summary>
        /// Draws a line from a to b.
        /// </summary>
        /// <param name="a">
        /// Origin is the top left corner of the element.
        /// </param>
        /// <param name="b">
        /// Origin is the top left corner of the element.
        /// </param>
        /// <returns></returns>
        public CanvasElement Draw(Point a, Point b)
        {
            this.GetDriver().CreateActions()
                .MoveToElement(this, a.X, a.Y)
                .ClickAndHold()
                .MoveToElement(this, b.X, b.Y)
                .Release(this)
                .Perform();

            return this;
        }

        #endregion
    }
}
