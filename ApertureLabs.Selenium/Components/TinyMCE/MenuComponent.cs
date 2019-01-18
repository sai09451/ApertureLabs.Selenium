using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// The menu component of a TinyMCEComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class MenuComponent : PageComponent, IBaseToolbar
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By itemsSelector = By.CssSelector(".mce-container-body *[role='menuitem']");
        private readonly By itemNameSelector = By.CssSelector(".mce-txt");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The pageObjectFactory.</param>
        /// <param name="driver">The driver.</param>
        public MenuComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> ItemElements => WrappedElement.FindElements(itemsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public MenuItem GetItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var el = FirstOrDefaultElement(itemName, stringComparison);

            if (el == null)
            {
                return null;
            }
            else
            {
                return pageObjectFactory.PrepareComponent(
                    new MenuItem(
                        WrappedDriver,
                        WrappedDriver.CreateCssSelectorElement(el)));
            }
        }

        public MenuItem GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified item name has item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        /// <c>true</c> if the specified item name has item; otherwise, <c>false</c>.
        /// </returns>
        public bool HasItemWithText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            return FirstOrDefaultElement(itemName, stringComparison) != null;
        }

        public bool HasItemWithClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        private IWebElement FirstOrDefaultElement(string itemName,
            bool isText,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            IWebElement result = null;

            foreach (var element in ItemElements)
            {
                if (isText)
                {
                    // Search for matching text.
                }
                else
                {
                    // Search for matching icon class.
                }

                var titleEl = element.FindElements(itemNameSelector)
                    .FirstOrDefault();

                if (titleEl == null)
                    continue;

                var matches = String.Equals(
                    itemName,
                    titleEl.TextHelper().InnerText,
                    stringComparison);

                if (matches)
                {
                    result = element;
                    break;
                }
            }

            return result;
        }

        #endregion
    }
}
