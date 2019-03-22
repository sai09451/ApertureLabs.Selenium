using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.Kendo.KListView
{
    /// <summary>
    /// Represents the k-list component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoComponent{T}" />
    public class KListViewComponent<T> : BaseKendoComponent<T>
    {
        #region Fields

        #region Selectors

        #endregion

        private readonly KListViewConfiguration configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KListViewComponent{T}"/> class.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="driver"></param>
        /// <param name="configuration"></param>
        /// <param name="parent">The parent.</param>
        public KListViewComponent(By selector,
            IWebDriver driver,
            KListViewConfiguration configuration,
            T parent)
            : base(configuration,
                  selector,
                  driver,
                  parent)
        {
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> RowElements => WrappedElement
            .FindElements(configuration.RowsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Enumerates the over all rows.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IWebElement> EnumerateOverAllRows()
        {
            return RowElements;
        }

        /// <summary>
        /// Gets the cell.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public virtual IWebElement GetCell(int row, int col)
        {
            return RowElements.ElementAt(row)
                .FindElements(configuration.ColsSelector)
                .ElementAt(col);
        }

        #endregion
    }
}
