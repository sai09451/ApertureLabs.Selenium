using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Common interface for TinyMCE components that have nested
    /// <see cref="MenuItem"/>s.
    /// </summary>
    public interface IGetMenuItem
    {
        /// <summary>
        /// Gets the menu item at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        MenuItem GetMenuItemAt(int index);

        /// <summary>
        /// Gets the menu items.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<MenuItem> GetMenuItems();

        /// <summary>
        /// Gets the item by text.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        MenuItem GetItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Gets the item by text and attempts to convert it to the type
        /// paramater.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        T GetItemByText<T>(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItem;

        /// <summary>
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        MenuItem GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Gets the item by class and attempts to convert it to the type
        /// parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        T GetItemByClass<T>(string className,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : MenuItem;
    }
}
