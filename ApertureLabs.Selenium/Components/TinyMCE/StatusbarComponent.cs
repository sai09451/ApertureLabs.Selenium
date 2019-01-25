using System;
using System.Collections.Generic;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Statusbar of a TinyMCEComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class StatusbarComponent : PageComponent, IBaseToolbar
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusbarComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        public StatusbarComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(driver, selector)
        {
            this.pageObjectFactory = pageObjectFactory;
        }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public MenuItem GetItemByClass(string className, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        public MenuItem GetItemByText(string itemName, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        public MenuItem GetMenuItemAt(int index)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<MenuItem> GetMenuItems()
        {
            throw new NotImplementedException();
        }

        public bool HasItemWithClass(string className, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        public bool HasItemWithText(string itemName, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
