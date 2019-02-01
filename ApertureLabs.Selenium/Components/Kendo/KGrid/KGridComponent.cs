using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Components.Kendo.KPager;
using ApertureLabs.Selenium.Components.Kendo.KToolbar;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.Kendo.KGrid
{
    /// <summary>
    /// Represents a kendo grid.
    /// </summary>
    public class KGridComponent : BaseKendoComponent
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By RowsSelector = By.CssSelector(".k-grid-content *[role='row']");
        private readonly By CellsSelector = By.CssSelector("*[role='gridcell']");
        private readonly By HeadersSelector = By.CssSelector("*[role='columnheader']");
        private By PagerSelector;
        private By ToolbarSelector;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="configuration"></param>
        public KGridComponent(BaseKendoConfiguration configuration,
            By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(configuration,
                  selector,
                  driver)
        {
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

        private IReadOnlyList<IWebElement> RowElements => WrappedElement.FindElements(RowsSelector);
        private IReadOnlyList<IWebElement> HeaderElements => WrappedElement.FindElements(HeadersSelector);

        /// <summary>
        /// The pager used to control the grid.
        /// </summary>
        public KPagerComponent Pager => pageObjectFactory.PrepareComponent(
            new KPagerComponent(
                configuration,
                PagerSelector,
                pageObjectFactory,
                WrappedDriver));

        /// <summary>
        /// Toolbar component.
        /// </summary>
        public KToolbarComponent Toolbar => pageObjectFactory.PrepareComponent(
            new KToolbarComponent(
                configuration,
                ToolbarSelector,
                WrappedDriver));

        #endregion

        #endregion

        #region Methods

        /// <inheritDoc/>
        public override ILoadableComponent Load()
        {
            base.Load();

            PagerSelector = ByScoped.FromScope(
                WrappedElement,
                new[] { By.CssSelector(".k-pager-wrap.k-grid-pager") });

            ToolbarSelector = ByScoped.FromScope(
                WrappedElement,
                new[] { By.CssSelector(".k-toolbar") });

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
        public IList<string> GetColumnHeaders()
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
        public IWebElement GetCell(int row, int col)
        {
            return RowElements
                .ElementAt(row)
                .FindElements(CellsSelector)
                .ElementAt(col);
        }

        /// <summary>
        /// NOTE: Throws an exception if this is a multi-header grid.
        /// </summary>
        /// <param name="columnHeaderName"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public IWebElement GetCell(string columnHeaderName, int row)
        {
            if (IsMultiColumnHeader)
            {
                throw new Exception("Can't call GetCell using the column " +
                    "header name with a multi column ");
            }

            var col = GetColumnHeaders().IndexOf(columnHeaderName);

            return GetCell(row, col);
        }

        #endregion
    }
}
