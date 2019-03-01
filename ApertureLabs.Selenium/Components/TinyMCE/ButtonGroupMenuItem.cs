using System;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// GroupedMenuItem.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.TinyMCE.MenuItem" />
    public class ButtonGroupMenuItem : MenuItem
    {
        #region Fields

        #region Selectors

        private readonly By subItemSelector = By.CssSelector("*[role='button']");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonGroupMenuItem"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        public ButtonGroupMenuItem(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(selector, pageObjectFactory, driver)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the sub item by text.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MenuItem GetSubItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            //var bttnEl = WrappedElement.FindElements(subItemSelector)
            //    .FirstOrDefault(el =>
            //    {
            //        var btns = el.FindElements();
            //        return false;
            //    });

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the sub item by icon.
        /// </summary>
        /// <param name="iconClass">The icon class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public MenuItem GetSubItemByIcon(string iconClass,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
