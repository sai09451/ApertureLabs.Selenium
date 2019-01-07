using ApertureLabs.Selenium.Components.Kendo.KDropDown;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.Kendo.KPager
{
    /// <summary>
    /// PagerComponent.
    /// </summary>
    public class KPagerComponent : BaseKendoComponent
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By FirstPageSelector = By.CssSelector(".k-pager-first");
        private readonly By PrevPageSelector = By.CssSelector("*[title='Go to the previous page']");
        private readonly By AvailablePagesSelector = By.CssSelector(".k-pager-numbers li:not(:last-child) > *");
        private readonly By SelectedPageSelector = By.CssSelector(".k-pager-numbers .k-state-selected");
        private readonly By MorePagesSelector = By.CssSelector(".k-pager-numbers li:last-child > *");
        private readonly By NextPageSelector = By.CssSelector("*[title='Go to the next page']");
        private readonly By LastPageSelector = By.CssSelector(".k-pager-last");
        private readonly By RefreshSelector = By.CssSelector(".k-pager-refresh");
        private readonly By PagerInfoSelector = By.CssSelector(".k-pager-info");
        private readonly By ItemsPerPageSelector;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        /// <param name="pageObjectFactory"></param>
        public KPagerComponent(IWebDriver driver,
            By selector,
            DataSourceOptions dataSourceOptions,
            IPageObjectFactory pageObjectFactory = default)
            : base(driver, selector, dataSourceOptions)
        {
            this.pageObjectFactory = pageObjectFactory
                ?? new PageObjectFactory();

            ItemsPerPageSelector = ScopedBy.FromScope(
                selector,
                By.CssSelector(".k-pager-sizes .k-dropdown select"));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if on the first page button is disabled.
        /// </summary>
        public bool IsOnFirstPage => FirstPageElement
            .Classes()
            .Contains("k-state-disabled");

        /// <summary>
        /// Returns false if the previous page button is disabled.
        /// </summary>
        public bool HasPreviousPage => !PrevPageElement
            .Classes()
            .Contains("k-state-disabled");

        /// <summary>
        /// Returns false if the next page button is disabled.
        /// </summary>
        public bool HasNextPage => !NextPageElement
            .Classes()
            .Contains("k-state-disabled");

        /// <summary>
        /// Returns true if the last page button is disabled.
        /// </summary>
        public bool IsOnLastPage => LastPageElement
            .Classes()
            .Contains("k-state-disabled");

        /// <summary>
        /// Used to determine if the current page range total items are displayed.
        /// </summary>
        public bool HasPageInfo => WrappedElement
            .FindElements(PagerInfoSelector)
            .Any();

        #region Elements

        private IWebElement FirstPageElement => WrappedElement.FindElement(FirstPageSelector);
        private IWebElement PrevPageElement => WrappedElement.FindElement(PrevPageSelector);
        private IReadOnlyList<IWebElement> AvailablePagesElements => WrappedElement.FindElements(AvailablePagesSelector);
        private IWebElement MorePagesElement => WrappedElement.FindElement(MorePagesSelector);
        private IWebElement NextPageElement => WrappedElement.FindElement(NextPageSelector);
        private IWebElement LastPageElement => WrappedElement.FindElement(LastPageSelector);
        private IWebElement RefreshElement => WrappedElement.FindElement(RefreshSelector);
        private IWebElement PageInfoElement => WrappedElement.FindElement(PagerInfoSelector);
        private IWebElement SelectedPageElement => WrappedElement.FindElement(SelectedPageSelector);

        private KDropDownComponent ItemsPerPageComponent =>
            pageObjectFactory.PrepareComponent(
                new KDropDownComponent(
                    WrappedDriver,
                    ItemsPerPageSelector,
                    dataSourceOptions,
                    KDropDownAnimationOptions.Default()));

        #endregion

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override ILoadableComponent Load()
        {
            base.Load();

            return this;
        }

        /// <summary>
        /// Sets the page the listed page with the matching number.
        /// </summary>
        /// <param name="listedPage"></param>
        /// <returns></returns>
        public virtual KPagerComponent SetPage(int listedPage)
        {
            var desiredPageEl = AvailablePagesElements
                .FirstOrDefault(
                    e => e.TextHelper().ExtractInteger() == listedPage);

            if (desiredPageEl == null)
            {
                throw new NoSuchElementException($"Failed to locate page " +
                    $"#{listedPage} among the available pages.");
            }

            // Click the page element, should trigger a loading indicator.
            desiredPageEl.Click();

            // Wait until the loading finishes.
            WaitForLoadingOperation();

            return this;
        }

        /// <summary>
        /// Returns a list of listed pages.
        /// </summary>
        /// <returns></returns>
        public virtual IList<int> GetListedPages()
        {
            return AvailablePagesElements
                .Select(e => e.TextHelper().ExtractInteger())
                .ToList();
        }

        /// <summary>
        /// Goes to the last page if available.
        /// </summary>
        /// <returns></returns>
        public virtual KPagerComponent LastPage()
        {
            if (!IsOnLastPage)
            {
                LastPageElement.Click();
                WaitForLoadingOperation();
            }

            return this;
        }

        /// <summary>
        /// Goes to the first page if available.
        /// </summary>
        /// <returns></returns>
        public KPagerComponent FirstPage()
        {
            if (!IsOnFirstPage)
            {
                FirstPageElement.Click();
                WaitForLoadingOperation();
            }

            return this;
        }

        /// <summary>
        /// Goes to the previous page if available, otherwise remains on the
        /// current page.
        /// </summary>
        /// <returns></returns>
        public virtual KPagerComponent PrevPage()
        {
            if (HasPreviousPage)
            {
                PrevPageElement.Click();
                WaitForLoadingOperation();
            }

            return this;
        }

        /// <summary>
        /// Goes to the next page if available.
        /// </summary>
        /// <returns></returns>
        public virtual KPagerComponent NextPage()
        {
            if (HasNextPage)
            {
                NextPageElement.Click();
                WaitForLoadingOperation();
            }

            return this;
        }

        /// <summary>
        /// Returns the active page number.
        /// </summary>
        /// <returns></returns>
        public virtual int GetActivePage()
        {
            return SelectedPageElement.TextHelper().ExtractInteger();
        }

        /// <summary>
        /// Returns the total items.
        /// </summary>
        /// <returns></returns>
        public virtual int GetTotalItems()
        {
            return PageInfoElement.TextHelper().ExtractIntegers().Last();
        }

        /// <summary>
        /// Retrieves the available items per page.
        /// </summary>
        /// <returns></returns>
        public virtual IList<int> GetAvailableItemsPerPage()
        {
            var itemsPerPage = ItemsPerPageComponent.GetSelectElement()
                .Options
                .Select(e => e.TextHelper().ExtractInteger())
                .ToList();

            return itemsPerPage;
        }

        /// <summary>
        /// Returns the selected value for items per page.
        /// </summary>
        /// <returns></returns>
        public virtual int GetItemsPerPage()
        {
            var selectedOpt = ItemsPerPageComponent.GetSelectElement()
                .SelectedOption
                .TextHelper()
                .ExtractInteger();

            return selectedOpt;
        }

        /// <summary>
        /// Sets the 'Items per Page'.
        /// </summary>
        /// <param name="itemsPerPage"></param>
        public virtual void SetItemsPerPage(int itemsPerPage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Refreshes the context the pager is attached to.
        /// </summary>
        public virtual void Refresh()
        {
            RefreshElement.Click();
            WaitForLoadingOperation();
        }

        #endregion
    }
}
