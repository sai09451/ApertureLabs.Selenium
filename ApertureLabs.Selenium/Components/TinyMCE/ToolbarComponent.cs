using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Toolbar component of the TinyMCEComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class ToolbarComponent : PageComponent, IBaseToolbar, IGetMenuItem
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By itemsSelector = By.CssSelector(".mce-container-body .mce-container");
        private readonly By itemNameSelector = By.CssSelector(".mce-txt");
        private readonly By itemIconSeletor = By.CssSelector(".mce-ico");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        public ToolbarComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(selector, driver)
        {
            this.pageObjectFactory = pageObjectFactory;
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> ItemElements => WrappedElement
            .FindElements(itemsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual MenuItemComponent GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var menuItemEl = ItemElements.FirstOrDefault(el =>
            {
                return el
                    .FindElements(itemIconSeletor)
                    .Any(
                        iconEl => iconEl.Classes().Any(
                            @class => String.Equals(
                                @class,
                                className,
                                stringComparison)));
            });

            if (menuItemEl == null)
                throw new NoSuchElementException();

            var cssSelector = WrappedDriver.GetCssSelector(menuItemEl);

            return pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    cssSelector,
                    pageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Gets the item by class and attempts to convert it to the type
        /// parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual T GetItemByClass<T>(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItemComponent
        {
            var item = GetItemByClass(className, stringComparison);

            return item.ConvertTo<T>();
        }

        /// <summary>
        /// Gets the item by text.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual MenuItemComponent GetItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var menuItemEl = ItemElements.FirstOrDefault(el =>
            {
                return el.FindElements(itemNameSelector)
                    .Any(name => String.Equals(
                        el.TextHelper().InnerText,
                        itemName,
                        stringComparison));
            });

            if (menuItemEl == null)
                throw new NoSuchElementException();

            var cssSelector = WrappedDriver.GetCssSelector(menuItemEl);

            return pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    cssSelector,
                    pageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Gets the item by text and attempts to convert it to the type
        /// paramater.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual T GetItemByText<T>(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItemComponent
        {
            var item = GetItemByText(itemName, stringComparison);

            return item.ConvertTo<T>();
        }

        /// <summary>
        /// Gets the menu item at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual MenuItemComponent GetMenuItemAt(int index)
        {
            var cssSelector = WrappedDriver.GetCssSelector(
                ItemElements.ElementAt(
                    index));

            var menuItem = pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    cssSelector,
                    pageObjectFactory,
                    WrappedDriver));

            return menuItem;
        }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IReadOnlyCollection<MenuItemComponent> GetMenuItems()
        {
            var menuItems = ItemElements
                .Select(e => pageObjectFactory.PrepareComponent(
                    new MenuItemComponent(
                        WrappedDriver.GetCssSelector(e),
                        pageObjectFactory,
                        WrappedDriver)))
                .ToList()
                .AsReadOnly();

            return menuItems;
        }

        /// <summary>
        /// Determines whether the item exists.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        /// <c>true</c> if [has item by class] [the specified class name]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasItemWithClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var menuItemEl = ItemElements.FirstOrDefault(el =>
            {
                return el
                    .FindElements(itemIconSeletor)
                    .Any(
                        iconEl => iconEl.Classes().Any(
                            @class => String.Equals(
                                @class,
                                className,
                                stringComparison)));
            });

            return menuItemEl != null;
        }

        /// <summary>
        /// Determines whether the item exists.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        /// <c>true</c> if the specified item name has item; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool HasItemWithText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var menuItemEl = ItemElements.FirstOrDefault(el =>
            {
                return el.FindElements(itemNameSelector)
                    .Any(name => String.Equals(
                        el.TextHelper().InnerText,
                        itemName,
                        stringComparison));
            });

            return menuItemEl != null;
        }

        #endregion
    }
}
