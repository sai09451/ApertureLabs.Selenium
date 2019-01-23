using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Represents a drop down style menu item.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.TinyMCE.MenuItem" />
    public class DropDownMenuItem : MenuItem
    {
        #region Fields

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownMenuItem"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        public DropDownMenuItem(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Opens the drop down and returns the option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MenuItem SelectOption(string option)
        {
            WrappedElement.Click();

            throw new NotImplementedException();
        }

        #endregion
    }
}
