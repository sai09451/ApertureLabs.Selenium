using System.Collections.Generic;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Menu item of an IBaseToolbar.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class MenuItem : PageComponent
    {
        #region Fields

        /// <summary>
        /// An ordered list of parent menu items.
        /// </summary>
        protected IList<MenuItem> parentMenuItems;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="parentMenuItems">The parent menu items.</param>
        public MenuItem(IWebDriver driver,
            By selector,
            IList<MenuItem> parentMenuItems = null)
            : base(driver, selector)
        {
            this.parentMenuItems = parentMenuItems ?? new List<MenuItem>();
        }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether this instance is displayed.
        /// </summary>
        public bool IsDisplayed()
        {
            return WrappedElement.Displayed;
        }

        /// <summary>
        /// Returns this as a new DropDownMenuItem.
        /// </summary>
        /// <returns></returns>
        public DropDownMenuItem AsDropDown()
        {
            return new DropDownMenuItem(WrappedDriver, By);
        }

        /// <summary>
        /// Returns the title of this menu item.
        /// </summary>
        /// <returns></returns>
        public string AsText()
        {
            return AsElement().TextHelper().InnerText;
        }

        /// <summary>
        /// Returns this as a new GroupedMenuItem.
        /// </summary>
        /// <returns></returns>
        public GroupedMenuItem AsGroupedMenuItem()
        {
            return new GroupedMenuItem(WrappedDriver, By);
        }

        /// <summary>
        /// Returns the WrappedElement.
        /// </summary>
        /// <returns></returns>
        public IWebElement AsElement()
        {
            return WrappedElement;
        }

        #endregion
    }
}
