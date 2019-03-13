using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components
{
    /// <summary>
    /// Represents a 'tabbable' component. To be used with
    /// <see cref="ILoadableComponent"/>
    /// </summary>
    public interface ITabbable
    {
        /// <summary>
        /// Gets the name of the active tab.
        /// </summary>
        /// <returns></returns>
        string GetActiveTabName();

        /// <summary>
        /// Gets the tab names.
        /// </summary>
        /// <returns></returns>
        IReadOnlyCollection<string> GetTabNames();

        /// <summary>
        /// Gets the active tab body. Null if no tabs are active.
        /// </summary>
        /// <returns></returns>
        IWebElement GetActiveTabBody();

        /// <summary>
        /// Gets the tab body of a tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException">
        /// Thrown if no tab body exists.
        /// </exception>
        IWebElement GetTabBody(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        void SelectTab(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Determines whether the specified tab name exists.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if the specified tab name exists; otherwise, <c>false</c>.
        /// </returns>
        bool HasTab(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal);

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <typeparam name="T">The type of the tab body component.</typeparam>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        T SelectTab<T>(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal)
            where T : class, ILoadableComponent;

        /// <summary>
        /// Gets the valid tab body types for use with
        /// <see cref="SelectTab{T}(string, StringComparison)"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Type> GetValidTabBodyTypes();
    }
}
