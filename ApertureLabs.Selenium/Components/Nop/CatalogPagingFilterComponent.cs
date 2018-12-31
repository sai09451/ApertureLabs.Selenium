//using OpenQA.Selenium;
//using OpenQA.Selenium.Support.UI;
//using System.Collections.Generic;

//namespace ApertureLabs.Selenium.Components.Nop
//{
//    /// <summary>
//    /// How items are displayed in a pager. Corresponds to
//    /// Nop.Web/Views/Catalog/_CatalogSelectors.cshtml
//    /// </summary>
//    public enum ViewMode
//    {
//        /// <summary>
//        /// Displays items in a grid.
//        /// </summary>
//        GridMode,

//        /// <summary>
//        /// Displays items in a list.
//        /// </summary>
//        ListMode
//    }

//    /// <summary>
//    /// CatalogPagingFilterComponent
//    /// </summary>
//    public class CatalogPagingFilterComponent : PagerComponent
//    {
//        #region Fields

//        #region Selectors

//        private readonly By gridModeSelector = By.CssSelector(".viewmode-icon.grid");
//        private readonly By listModeSelector = By.CssSelector(".viewmode-icon.list");
//        private readonly By productSortingSelector = By.CssSelector("#products-orderby");
//        private readonly By pageSizeSelector = By.CssSelector("#products-pagesize");

//        #endregion

//        #endregion

//        #region Constructor

//        /// <summary>
//        /// Uses the '.product-selectors' as the selector to get the element.
//        /// </summary>
//        /// <param name="driver"></param>
//        public CatalogPagingFilterComponent(IWebDriver driver)
//            : base(driver, By.CssSelector(".product-selectors"))
//        { }

//        /// <summary>
//        /// Gets the element the selector.
//        /// </summary>
//        /// <param name="driver"></param>
//        /// <param name="selector"></param>
//        public CatalogPagingFilterComponent(IWebDriver driver, By selector)
//            : base(driver, selector)
//        { }

//        #endregion

//        #region Properties

//        #region Elements

//        private IWebElement GridModeElement => WrappedElement.FindElement(gridModeSelector);
//        private IWebElement ListModeElement => WrappedElement.FindElement(listModeSelector);
//        private SelectElement ProductSortingElement => new SelectElement(WrappedElement.FindElement(productSortingSelector));
//        private SelectElement PageSizeElement => new SelectElement(WrappedElement.FindElement(pageSizeSelector));

//        #endregion

//        /// <inheritdoc/>
//        public IList<IWebElement> SortOptions => ProductSortingElement.Options;

//        /// <inheritdoc/>
//        public IWebElement SortedBy => ProductSortingElement.SelectedOption;

//        /// <inheritdoc/>
//        public IList<IWebElement> PageSizeOptions => PageSizeElement.Options;

//        /// <inheritdoc/>
//        public IWebElement PageSize => PageSizeElement.SelectedOption;

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Clicks grid mode.
//        /// </summary>
//        public void UseGridMode()
//        {
//            GridModeElement.Click();
//        }

//        /// <summary>
//        /// Clicks list mode.
//        /// </summary>
//        public void UseListMode()
//        {
//            ListModeElement.Click();
//        }

//        /// <summary>
//        /// Selects the option by index.
//        /// </summary>
//        /// <param name="optionIndex"></param>
//        public void SortByIndex(int optionIndex)
//        {
//            ProductSortingElement.SelectByIndex(optionIndex);
//        }

//        /// <summary>
//        /// Selects the option by its text.
//        /// </summary>
//        /// <param name="optionText"></param>
//        public void SortByText(string optionText)
//        {
//            ProductSortingElement.SelectByText(optionText);
//        }

//        /// <summary>
//        /// Selects the option by value.
//        /// </summary>
//        /// <param name="optionValue"></param>
//        public void SortByValue(string optionValue)
//        {
//            ProductSortingElement.SelectByValue(optionValue);
//        }

//        #endregion
//    }
//}
