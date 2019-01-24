using System;
using System.Linq;
using System.Threading;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

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

        private readonly By allMenuDropDownsSelector = By.CssSelector(".mce-panel.mce-floatpanel.mce-menu.mce-animate.mce-menu-has-icons.mce-menu-align");
        private readonly By dropDownItemsSelector = By.CssSelector("*[role='menuitem']");
        private readonly By textSelector = By.CssSelector(".mce-text");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownMenuItem"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        public DropDownMenuItem(By selector,
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
        /// Opens the drop down and returns the option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MenuItem SelectOption(string option,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            // This isn't ideal but since the dropdown menu items are 
            // generated on the first time the parent menu item is clicked,
            // click the parent element, wait 500 ms (should be enough time for
            // the dropdown to be generated), and move the mouse below the
            // parent element to locate the dropdown container.
            WrappedElement.Click();
            Thread.Sleep(500);

            // Move below the WrappedElement.
            WrappedDriver.CreateActions()
                .MoveToElement(WrappedElement,
                    0,
                    WrappedElement.Size.Height,
                    MoveToElementOffsetOrigin.Center)
                .Perform();

            // Get the container element under the mouse.
            var focusedElement = WrappedDriver
                .GetHoveredElements(allMenuDropDownsSelector)
                .First();

            var menuItemEl = focusedElement.FindElements(dropDownItemsSelector)
                .FirstOrDefault(el =>
                {
                    var textEl = el.FindElements(textSelector)
                        .FirstOrDefault();

                    if (textEl == null)
                        return false;

                    return String.Equals(
                        textEl.TextHelper().InnerText,
                        option,
                        stringComparison);
                });

            if (menuItemEl == null)
                throw new NoSuchElementException();

            var selector = ByElement.FromElement(menuItemEl);

            return pageObjectFactory.PrepareComponent(
                new MenuItem(
                    selector,
                    pageObjectFactory,
                    WrappedDriver));
        }

        #endregion
    }
}
