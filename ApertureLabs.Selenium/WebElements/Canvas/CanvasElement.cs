using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.WebElements.Canvas
{
    /// <summary>
    /// CanvasElement.
    /// </summary>
    public class CanvasElement : BaseElement
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public CanvasElement(IWebElement element) : base(element)
        {
            if (!String.Equals(element.TagName,
                "canvas",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new UnexpectedTagNameException("Expected tagname to be 'canvas'.");
            }
        }

        #endregion

        #region Methods

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
