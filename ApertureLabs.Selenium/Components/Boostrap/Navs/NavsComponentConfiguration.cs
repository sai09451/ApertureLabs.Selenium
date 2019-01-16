using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Navs
{
    /// <summary>
    /// Options for configuring the NavsComponent. All selectors are relative
    /// to WrappedElement of the NavComponent.
    /// </summary>
    public class NavsComponentConfiguration
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
    }
}
