using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Components.Kendo.KPager;
using ApertureLabs.Selenium.Components.Kendo.KToolbar;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.Kendo.KGrid
{
    /// <summary>
    /// Represents a kendo grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KGridComponent<T> : BaseKendoComponent<T>
    {
        #region Fields

        #region Selectors

        private readonly By rowsSelector = By.CssSelector("tbody *[role='row']");
        private readonly By cellsSelector = By.CssSelector("*[role='gridcell']");
        private readonly By headersSelector = By.CssSelector("*[role='columnheader']");
        private By pagerSelector;
        private By toolbarSelector;

        #endregion

        private readonly BaseKendoConfiguration configuration;
        private readonly IPageObjectFactory pageObjectFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parent">The parent.</param>
        public KGridComponent(BaseKendoConfiguration configuration,
            By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver,
            T parent)
            : base(configuration,
                  selector,
                  driver,
                  parent)
        {
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
            this.pageObjectFactory = pageObjectFactory
                ?? throw new ArgumentNullException(nameof(pageObjectFactory));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Used to check for nested headers.
        /// </summary>
        protected bool IsMultiColumnHeader { get; private set; }

        #region Elements

        private IReadOnlyList<IWebElement> RowElements => WrappedElement.FindElements(rowsSelector);
        private IReadOnlyList<IWebElement> HeaderElements => WrappedElement.FindElements(headersSelector);

        /// <summary>
        /// The pager used to control the grid.
        /// </summary>
        public virtual KPagerComponent<KGridComponent<T>> Pager { get; private set; }

        /// <summary>
        /// Toolbar component.
        /// </summary>
        public virtual KToolbarComponent<KGridComponent<T>> Toolbar { get; private set; }

        /// <summary>
        /// Enumerates over all rows. This will first navigate to the first
        /// page then being yielding rows. Unless the enumerator is stopped, it
        /// will end on the last page.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<IWebElement> EnumerateOverAllRows()
        {
            // Go to the first page.
            Pager.FirstPage();

            do
            {
                var numberOfRows = GetNumberOfRows();

                for (var i = 0; i < numberOfRows; i++)
                {
                    var row = GetRow(i);

                    yield return row;
                }

                Pager.NextPage();

            } while (Pager.HasNextPage);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// If overriding don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        public override ILoadableComponent Load()
        {
            base.Load();

            pagerSelector = new ByChained(
                new ByElement(WrappedElement),
                By.CssSelector(".k-pager-wrap.k-grid-pager"));

            toolbarSelector = new ByChained(
                new ByElement(WrappedElement),
                By.CssSelector(".k-toolbar"));

            if (WrappedDriver.FindElements(toolbarSelector).Any())
            {
                Toolbar = new KToolbarComponent<KGridComponent<T>>(
                    configuration,
                    toolbarSelector,
                    WrappedDriver,
                    this);

                pageObjectFactory.PrepareComponent(Toolbar);
            }
            else
            {
                Toolbar = null;
            }

            if (WrappedDriver.FindElements(pagerSelector).Any())
            {
                Pager = new KPagerComponent<KGridComponent<T>>(
                    configuration,
                    pagerSelector,
                    pageObjectFactory,
                    WrappedDriver,
                    this);

                pageObjectFactory.PrepareComponent(Pager);
            }
            else
            {
                Pager = null;
            }

            // Check for multi-column headers.
            var theadRows = WrappedElement
                .FindElements(By.CssSelector("thead > tr"))
                .ToList();

            IsMultiColumnHeader = theadRows.Count > 1;

            return this;
        }

        /// <summary>
        /// Returns a list of all column names.
        /// </summary>
        /// <returns></returns>
        public virtual IList<string> GetColumnHeaders()
        {
            return HeaderElements
                .Select(e => e.TextHelper().InnerText)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Returns the element cell.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public virtual IWebElement GetCell(int row, int col)
        {
            return RowElements
                .ElementAt(row)
                .FindElements(cellsSelector)
                .ElementAt(col);
        }

        /// <summary>
        /// NOTE: Throws an exception if this is a multi-header grid.
        /// </summary>
        /// <param name="columnHeaderName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public virtual IWebElement GetCell(string columnHeaderName, int row)
        {
            if (IsMultiColumnHeader)
            {
                throw new Exception("Can't call GetCell using the column " +
                    "header name with a multi column ");
            }

            var col = GetColumnHeaders().IndexOf(columnHeaderName);

            return GetCell(row, col);
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <returns></returns>
        public virtual int GetNumberOfColumns()
        {
            return HeaderElements.Count;
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <returns></returns>
        public virtual int GetNumberOfRows()
        {
            return RowElements.Count;
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        public virtual IWebElement GetRow(int row)
        {
            return RowElements.ElementAt(row);
        }

        #endregion
    }
}
