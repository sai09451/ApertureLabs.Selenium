using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Navs
{
    /// <summary>
    /// Options for configuring the NavsComponent. All selectors are relative
    /// to WrappedElement of the NavComponent.
    /// </summary>
    public class NavsTabComponentConfiguration
    {
        /// <summary>
        /// Gets or sets the tab header elements selector.
        /// </summary>
        /// <value>
        /// The tab header elements selector.
        /// </value>
        public By TabHeaderElementsSelector { get; set; }

        /// <summary>
        /// Gets or sets the tab header names selector.
        /// </summary>
        /// <value>
        /// The tab header names selector.
        /// </value>
        public By TabHeaderNamesSelector { get; set; }

        /// <summary>
        /// Gets or sets the active tab header element selector.
        /// </summary>
        /// <value>
        /// The active tab header element selector.
        /// </value>
        public By ActiveTabHeaderElementSelector { get; set; }

        /// <summary>
        /// Gets or sets the active tab header name selector.
        /// </summary>
        /// <value>
        /// The active tab header name selector.
        /// </value>
        public By ActiveTabHeaderNameSelector { get; set; }

        /// <summary>
        /// Gets or sets the active tab content element selector.
        /// </summary>
        /// <value>
        /// The active tab content element selector.
        /// </value>
        public By ActiveTabContentElementSelector { get; set; }

        /// <summary>
        /// Default bootstrap selectors.
        /// </summary>
        /// <returns></returns>
        public static NavsTabComponentConfiguration Default()
        {
            return new NavsTabComponentConfiguration
            {
                ActiveTabContentElementSelector = By.CssSelector(".tab-content .tab-pane.active"),
                ActiveTabHeaderElementSelector = By.CssSelector(".nav-tabs .nav-link.active"),
                ActiveTabHeaderNameSelector = By.CssSelector(".nav-tabs .nav-link.active"),
                TabHeaderElementsSelector = By.CssSelector(".nav-tabs .nav-link"),
                TabHeaderNamesSelector = By.CssSelector(".nav-tabs .nav-link")
            };
        }
    }
}
