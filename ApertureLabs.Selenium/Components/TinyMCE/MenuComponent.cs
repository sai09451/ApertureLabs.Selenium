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
        private readonly By itemIconSeletor = By.CssSelector(".mce-ico");

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
        {
            this.pageObjectFactory = pageObjectFactory;
        }

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
            var el = FirstOrDefaultElement(itemName, true, stringComparison);

            if (el == null)
            {
                return null;
            }
            else
            {
                return pageObjectFactory.PrepareComponent(
                    new MenuItem(
                        WrappedDriver.GetCssSelector(el),
                        pageObjectFactory,
                        WrappedDriver));
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public MenuItem GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var el = FirstOrDefaultElement(className, false, stringComparison);

            if (el == null)
            {
                return null;
            }
            else
            {
                return pageObjectFactory.PrepareComponent(
                    new MenuItem(
                        WrappedDriver.GetCssSelector(el),
                        pageObjectFactory,
                        WrappedDriver));
            }
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
            return FirstOrDefaultElement(itemName, true, stringComparison) != null;
        }

        /// <summary>
        /// Determines if the menu item exists.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public bool HasItemWithClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            return FirstOrDefaultElement(className, false, stringComparison) != null;
        }

        private IWebElement FirstOrDefaultElement(string itemName,
            bool isText,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            IWebElement result = null;

            foreach (var element in ItemElements)
            {
                var matches = false;

                if (isText)
                {
                    // Search for matching text.
                    var titleEl = element.FindElements(itemNameSelector)
                        .FirstOrDefault();

                    if (titleEl == null)
                        continue;

                    matches = String.Equals(
                        itemName,
                        titleEl.TextHelper().InnerText,
                        stringComparison);
                }
                else
                {
                    // Search for matching icon class.
                    var iconEl = element.FindElements(itemIconSeletor)
                        .FirstOrDefault();

                    if (iconEl == null)
                        continue;

                    matches = iconEl.Classes().Any(c => String.Equals(
                        c,
                        itemName,
                        stringComparison));
                }

                if (matches)
                {
                    result = element;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<MenuItem> GetMenuItems()
        {
            var items = ItemElements.Select(e => pageObjectFactory.PrepareComponent(
                    new MenuItem(
                        WrappedDriver.GetCssSelector(e),
                        pageObjectFactory,
                        WrappedDriver)))
                .ToList()
                .AsReadOnly();

            return items;
        }

        /// <summary>
        /// Gets the menu item at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public MenuItem GetMenuItemAt(int index)
        {
            var menuItem = pageObjectFactory.PrepareComponent(
                new MenuItem(
                    WrappedDriver.GetCssSelector(ItemElements.ElementAt(index)),
                    pageObjectFactory,
                    WrappedDriver));

            return menuItem;
        }

        #endregion
    }
}
