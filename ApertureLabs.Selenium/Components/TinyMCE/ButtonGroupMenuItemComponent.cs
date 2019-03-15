using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// GroupedMenuItem.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.TinyMCE.MenuItemComponent" />
    public class ButtonGroupMenuItemComponent : MenuItemComponent, IGetMenuItem
    {
        #region Fields

        #region Selectors

        private readonly By subItemSelector = By.CssSelector("*[role='button']");

        #endregion

        private readonly IPageObjectFactory pageObjectFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtonGroupMenuItemComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        public ButtonGroupMenuItemComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(selector, pageObjectFactory, driver)
        {
            this.pageObjectFactory = pageObjectFactory
                ?? throw new ArgumentNullException(nameof(pageObjectFactory));
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> SubItemElements => WrappedElement
            .FindElements(subItemSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the menu item at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public virtual MenuItemComponent GetMenuItemAt(int index)
        {
            var menuItemEl = SubItemElements.ElementAt(index);
            var selector = WrappedDriver.GetCssSelector(menuItemEl);

            return pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    selector,
                    pageObjectFactory,
                    WrappedDriver));
        }

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<MenuItemComponent> GetMenuItems()
        {
            return SubItemElements.Select(
                    item => pageObjectFactory.PrepareComponent(
                        new MenuItemComponent(
                            WrappedDriver.GetCssSelector(item),
                            pageObjectFactory,
                            WrappedDriver)))
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Gets the item by text.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public virtual MenuItemComponent GetItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var bttnEl = SubItemElements.FirstOrDefault(el =>
            {
                return String.Equals(
                    itemName,
                    el.TextHelper().InnerText,
                    stringComparison);
            });

            if (bttnEl == null)
                throw new NoSuchElementException();

            return pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    new ByElement(bttnEl),
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
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public virtual MenuItemComponent GetItemByClass(string className, StringComparison stringComparison = StringComparison.Ordinal)
        {
            var bttnEl = SubItemElements.FirstOrDefault(bttn =>
            {
                // Try and the icon element.
                var iconEls = bttn.FindElements(By.CssSelector(".mce-ico"));

                foreach (var iconEl in iconEls)
                {
                    var iconClasses = iconEl.Classes();

                    foreach (var @class in iconClasses)
                    {
                        var hasIcon = String.Equals(
                            className,
                            @class,
                            stringComparison);

                        if (hasIcon)
                            return true;
                    }
                }

                // Failed to find the sub item.
                return false;
            });

            if (bttnEl == null)
                throw new NoSuchElementException();

            return pageObjectFactory.PrepareComponent(
                new MenuItemComponent(
                    new ByElement(bttnEl),
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

        #endregion
    }
}
