using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Defines methods for retrieving items.
    /// </summary>
    public interface IBaseToolbar
    {
        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<MenuItem> GetMenuItems();

        /// <summary>
        /// Gets the item by text.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        MenuItem GetItemByText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Gets the item by class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        MenuItem GetItemByClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Determines whether the item exists.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if the specified item name has item; otherwise, <c>false</c>.
        /// </returns>
        bool HasItemWithText(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Determines whether the item exists.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if [has item by class] [the specified class name]; otherwise, <c>false</c>.
        /// </returns>
        bool HasItemWithClass(string className,
            StringComparison stringComparison = StringComparison.Ordinal);
    }
}
