using System;
using System.Linq;
using System.Threading;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
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

            // Determine where the float-menu will appear. Usually it's either
            // directly below or to the right.
            var (X, Y) = GetDirectionFloatMenuWillOpen();

            // Move below the WrappedElement.
            WrappedDriver.CreateActions()
                .MoveToElement(WrappedElement,
                    Y,
                    X,
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

                    return textEl == null
                        ? false
                        : String.Equals(
                            textEl.TextHelper().InnerText,
                            option,
                            stringComparison);
                });

            if (menuItemEl == null)
                throw new NoSuchElementException();

            var selector = ByElement.FromElement(menuItemEl);

            return PageObjectFactory.PrepareComponent(
                new MenuItem(
                    selector,
                    PageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Selects the option and attempts to cast to the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option">The option.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual T SelectOption<T>(string option,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItem
        {
            var item = SelectOption(option, stringComparison);

            return item.ConvertTo<T>();
        }

        /// <summary>
        /// Gets the direction float menu will open relative to the CENTER of
        /// this WrappedElement.
        /// </summary>
        /// <returns></returns>
        private (int X, int Y) GetDirectionFloatMenuWillOpen()
        {
            var vector = (X: 0, Y: 0);

            if (WrappedElement.Classes().Contains("mce-button"))
            {
                // Opens to the right if enough space. Need to check if there
                // is enough space. TODO: identify way to check the offset of
                // the container due to lack of space.
                vector.X = WrappedElement.Size.Width;
                vector.Y = 0;
            }
            else
            {
                // Opens below.
                vector.X = 0;
                vector.Y = WrappedElement.Size.Height * -1;
            }

            return vector;
        }

        #endregion
    }
}
