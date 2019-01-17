using System;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Defines methods for retrieving items.
    /// </summary>
    public interface IBaseToolbar
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        MenuItem GetItem(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Determines whether the specified item name has item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if the specified item name has item; otherwise, <c>false</c>.
        /// </returns>
        bool HasItem(string itemName,
            StringComparison stringComparison = StringComparison.Ordinal);


    }
}
