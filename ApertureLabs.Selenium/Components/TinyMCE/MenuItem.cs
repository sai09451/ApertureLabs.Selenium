using System;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Menu item of a TinyMCEComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class MenuItem : PageComponent
    {
        #region Fields

        #region Selectors

        private readonly By iconSelector = By.CssSelector(".mce-ico");
        private readonly By textSelector = By.CssSelector(".mce-text");
        private readonly By shortcutSelector = By.CssSelector(".mce-menu-shortcut");
        private readonly By caretSelector = By.CssSelector(".mce-caret");

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
            : base(selector, driver)
        {
            PageObjectFactory = pageObjectFactory;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Page object factory.
        /// </summary>
        protected IPageObjectFactory PageObjectFactory { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is drop down.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is drop down; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDown => CaretElement != null;

        /// <summary>
        /// Gets a value indicating whether this instance is button group.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is button group; otherwise, <c>false</c>.
        /// </value>
        public bool IsButtonGroup => WrappedElement.Classes().Contains("mce-btn-group");

        /// <summary>
        /// Gets a value indicating whether this instance has title.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has title; otherwise, <c>false</c>.
        /// </value>
        public bool HasTitle => TextElement?.Displayed ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance has icon.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has icon; otherwise, <c>false</c>.
        /// </value>
        public bool HasIcon => !IconElement?.Classes().Contains("mce-none") ?? false;

        /// <summary>
        /// Gets a value indicating whether this instance has shortcut.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has shortcut; otherwise, <c>false</c>.
        /// </value>
        public bool HasShortcut => ShortCutElement?.Displayed ?? false;

        #region Elements

        /// <summary>
        /// Gets the icon element. Won't throw an error if the element doens't
        /// exist.
        /// </summary>
        protected IWebElement IconElement => WrappedElement.FindElements(iconSelector).FirstOrDefault();

        /// <summary>
        /// Gets the text element. Won't throw an error if the element doens't
        /// exist.
        /// </summary>
        protected IWebElement TextElement => WrappedElement.FindElements(textSelector).FirstOrDefault();

        /// <summary>
        /// Gets the shortcut element. Won't throw an error if the element
        /// doens't exist.
        /// </summary>
        protected IWebElement ShortCutElement => WrappedElement.FindElements(shortcutSelector).FirstOrDefault();

        /// <summary>
        /// Gets the caret element. Won't throw an error if the element doens't
        /// exist.
        /// </summary>
        protected IWebElement CaretElement => WrappedElement.FindElements(caretSelector).FirstOrDefault();

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Converts the item to a the derived MenuItem. Will return null if
        /// it fails to convert this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual T ConvertTo<T>() where T : MenuItem
        {
            var convertToType = typeof(T);
            var result = default(T);

            if (convertToType == typeof(DropDownMenuItem))
            {
                if (IsDropDown)
                {
                    result = PageObjectFactory.PrepareComponent(
                        new DropDownMenuItem(
                            By,
                            PageObjectFactory,
                            WrappedDriver)) as T;
                }
            }
            else if (convertToType == typeof(ButtonGroupMenuItem))
            {
                if (IsButtonGroup)
                {
                    result = PageObjectFactory.PrepareComponent(
                        new ButtonGroupMenuItem(
                            By,
                            PageObjectFactory,
                            WrappedDriver)) as T;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return PageObjectFactory.PrepareComponent(result);
        }

        /// <summary>
        /// Determines whether this instance is displayed.
        /// </summary>
        public bool IsDisplayed()
        {
            return WrappedElement.Displayed;
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
