using System;
using System.Collections.Generic;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Statusbar of a TinyMCEComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class StatusbarComponent : PageComponent, IBaseToolbar
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusbarComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        public StatusbarComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(selector, driver)
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
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual MenuItemComponent GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
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
            var item = GetItemByClass(className);

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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IReadOnlyCollection<MenuItemComponent> GetMenuItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the item exists.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        /// <c>true</c> if [has item by class] [the specified class name]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual bool HasItemWithClass(string className, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
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
        public virtual bool HasItemWithText(string itemName, StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
