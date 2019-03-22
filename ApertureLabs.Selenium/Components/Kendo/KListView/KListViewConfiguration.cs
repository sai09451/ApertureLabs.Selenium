using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Components.Kendo.KListView
{
    /// <summary>
    /// Configuration class for <see cref="KListViewComponent{T}"/>.
    /// </summary>
    /// <seealso cref="BaseKendoConfiguration"/>
    public class KListViewConfiguration : BaseKendoConfiguration
    {
        /// <summary>
        /// Gets or sets the rows selector.
        /// </summary>
        /// <value>
        /// The row selector.
        /// </value>
        public By RowsSelector { get; set; }

        /// <summary>
        /// Gets or sets the cols selector. This should be relative to the row
        /// element.
        /// </summary>
        /// <value>
        /// The col selector.
        /// </value>
        public By ColsSelector { get; set; }
    }
}
