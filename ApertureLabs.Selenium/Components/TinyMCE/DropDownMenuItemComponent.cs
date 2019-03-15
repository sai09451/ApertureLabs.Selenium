using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using ApertureLabs.Selenium.Css;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Represents a drop down style menu item.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.TinyMCE.MenuItemComponent" />
    public class DropDownMenuItemComponent : MenuItemComponent
    {
        #region Fields

        #region Selectors

        private readonly By allMenuDropDownsSelector = By.CssSelector(".mce-panel.mce-floatpanel.mce-menu.mce-menu-align");
        private readonly By dropDownItemsSelector = By.CssSelector("*[role='menuitem']");
        private readonly By textSelector = By.CssSelector(".mce-text");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownMenuItemComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        public DropDownMenuItemComponent(By selector,
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
        /// <param name="optionText">The option.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public virtual MenuItemComponent SelectOption(string optionText,
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
                .MoveToElement(
                    toElement: WrappedElement,
                    offsetX: X,
                    offsetY: Y,
                    offsetOrigin: MoveToElementOffsetOrigin.Center)
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
                            optionText,
                            stringComparison);
                });

            if (menuItemEl == null)
                throw new NoSuchElementException();

            var selector = ByElement.FromElement(menuItemEl);

            return PageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    selector,
                    PageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Selects the option and attempts to cast to the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemText">The item text.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual T SelectOption<T>(string itemText,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItemComponent
        {
            var item = SelectOption(itemText, stringComparison);

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

            var caretElement = WrappedElement.FindElement(
                By.CssSelector(".mce-caret"));

            var css = new CssColor(
                caretElement.GetCssValue(
                    "border-left-color"));

            // If the border-left-color is transparent, then it opens down.
            if (css.Color.ToArgb() == 0)
            {
                // Opens below.
                vector.X = 0;
                vector.Y = WrappedElement.Size.Height;
            }
            else
            {
                // Opens to the right if enough space. Need to check if there
                // is enough space. TODO: identify way to check the offset of
                // the container due to lack of space.
                vector.X = WrappedElement.Size.Width;
                vector.Y = 0;
            }

            return vector;
        }

        #endregion
    }
}
