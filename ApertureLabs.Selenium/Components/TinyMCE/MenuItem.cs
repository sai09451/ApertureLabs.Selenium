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

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        public MenuItem(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

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
