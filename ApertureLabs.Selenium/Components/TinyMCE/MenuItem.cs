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
        /// Page object factory.
        /// </summary>
        protected IPageObjectFactory pageObjectFactory;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuItem"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        public MenuItem(
            By selector,
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
            return pageObjectFactory.PrepareComponent(
                new DropDownMenuItem(
                    By,
                    pageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Returns the text of this menu item.
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
        public ButtonGroupMenuItem AsButtonGroupMenuItem()
        {
            return pageObjectFactory.PrepareComponent(
                new ButtonGroupMenuItem(
                    By,
                    pageObjectFactory,
                    WrappedDriver));
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
