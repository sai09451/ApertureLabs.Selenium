using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Navs
{
    /// <summary>
    /// Used for working with Bootstrap navs.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class NavsComponent : PageComponent
    {
        #region Fields

        private readonly NavsComponentConfiguration configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NavsComponent"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="configuration"></param>
        public NavsComponent(By selector,
            IWebDriver driver,
            NavsComponentConfiguration configuration)
            : base(driver, selector)
        {
            this.configuration = configuration;
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> TabHeaderNameElements => WrappedDriver.FindElements(configuration.TabHeaderNamesSelector);
        private IReadOnlyCollection<IWebElement> TabHeaderElements => WrappedElement.FindElements(configuration.TabHeaderElementsSelector);
        private IWebElement ActiveTabContentElement => WrappedElement.FindElement(configuration.ActiveTabContentElementSelector);
        private IWebElement ActiveTabHeaderElement => WrappedElement.FindElement(configuration.ActiveTabHeaderElementSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the tab names.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetTabNames()
        {
            return WrappedElement
                .FindElements(configuration.TabHeaderNamesSelector)
                .Select(e => e.TextHelper().InnerText)
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Gets the tab header elements.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IWebElement> GetTabHeaderElements()
        {
            return TabHeaderElements;
        }

        /// <summary>
        /// Gets the active tab body.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IWebElement GetActiveTabBody()
        {
            return ActiveTabContentElement;
        }

        /// <summary>
        /// Gets the name of the active tab.
        /// </summary>
        /// <returns></returns>
        public string GetActiveTabName()
        {
            return WrappedElement
                .FindElement(configuration.ActiveTabHeaderNameSelector)
                .TextHelper()
                .InnerText;
        }

        /// <summary>
        /// Sets the active tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        public void SetActiveTab(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var alreadyIsActive = String.Equals(
                tabName,
                GetActiveTabName(),
                stringComparison);

            if (alreadyIsActive)
                return;

            var tab = TabHeaderNameElements.FirstOrDefault(
                e => String.Equals(
                    tabName,
                    e.TextHelper().InnerText,
                    stringComparison));

            tab.Click();
        }

        #endregion
    }
}
