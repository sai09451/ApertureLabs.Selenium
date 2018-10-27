using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.Nop
{
    /// <summary>
    /// PagerComponent. Corresponds to Nop.Web.Framework.UI.Paging.Pager.
    /// </summary>
    public class PagerComponent : PageComponent
    {
        #region Fields

        #region Selectors

        private readonly By totalSummarySelector = By.CssSelector(".total-summary");
        private readonly By firstPageSelector = By.CssSelector(".first-page");
        private readonly By previousPageSelector = By.CssSelector(".previous-page");
        private readonly By currentPageSelector = By.CssSelector(".current-page");
        private readonly By individualPageSelector = By.CssSelector(".individual-page");
        private readonly By nextPageSelector = By.CssSelector(".next-page");
        private readonly By lastPageSelector = By.CssSelector(".last-page");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Uses the '.pager' selector to locate the component.
        /// </summary>
        /// <param name="driver"></param>
        public PagerComponent(IWebDriver driver) 
            : base(driver, By.CssSelector(".pager"))
        { }

        /// <summary>
        /// Can pass in a custom selector in case there are multiple pagers
        /// displayed.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public PagerComponent(IWebDriver driver,
            By selector): base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        private IWebElement TotalSummaryElement => WrappedElement.FindElement(totalSummarySelector);
        private IWebElement FirstPageElement => WrappedElement.FindElement(firstPageSelector);
        private IWebElement PreviousPageElement => WrappedElement.FindElement(previousPageSelector);
        private IWebElement CurrentElement => WrappedElement.FindElement(currentPageSelector);
        private IReadOnlyCollection<IWebElement> IndividualPageElements => WrappedElement.FindElements(individualPageSelector);
        private IWebElement NextPageElement => WrappedElement.FindElement(nextPageSelector);
        private IWebElement LastPageElement => WrappedElement.FindElement(lastPageSelector);

        #endregion

        /// <summary>
        /// The current page.
        /// </summary>
        public int CurrentPage => CurrentElement.TextHelper().ExtractInteger();

        /// <summary>
        /// Checks if there are previous results.
        /// </summary>
        public bool HasPrevious => WrappedElement
            .FindElements(previousPageSelector)
            .Any();

        /// <summary>
        /// Checks if there are next results.
        /// </summary>
        public bool HasNext => WrappedElement
            .FindElements(nextPageSelector)
            .Any();

        /// <summary>
        /// Gets a list of displayed page numbers.
        /// </summary>
        public IEnumerable<int> IndividualPages => IndividualPageElements
            .Select(p => p.GetTextHelper().ExtractInteger());

        #endregion

        #region Methods

        /// <summary>
        /// IsStale.
        /// </summary>
        /// <returns></returns>
        public override bool IsStale()
        {
            try
            {
                WrappedElement.GetAttribute("fake-attribute");
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Goes to the first result.
        /// </summary>
        public void First()
        {
            FirstPageElement.Click();
        }

        /// <summary>
        /// Goes to the previous result.
        /// </summary>
        public void Previous()
        {
            PreviousPageElement.Click();
        }

        /// <summary>
        /// Clicks on the page by the number it's displaying.
        /// </summary>
        /// <param name="pageNumber"></param>
        public void GoToPage(int pageNumber)
        {
            IndividualPageElements
                .First(page => pageNumber == page.GetTextHelper().ExtractInteger())
                .Click();
        }

        /// <summary>
        /// Goes to the next result.
        /// </summary>
        public void Next()
        {
            NextPageElement.Click();
        }

        /// <summary>
        /// Goes to the last result.
        /// </summary>
        public void Last()
        {
            LastPageElement.Click();
        }

        #endregion
    }
}
