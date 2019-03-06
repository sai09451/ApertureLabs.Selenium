using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.Boostrap.Navs
{
    /// <summary>
    /// Used for working with Bootstrap navs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.FluidPageComponent{T}" />
    public class NavsTabComponent<T> : FluidPageComponent<T>,
        ITabbable
    {
        #region Fields

        private readonly NavsTabComponentConfiguration configuration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NavsTabComponent{T}"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="configuration"></param>
        /// <param name="parent">The parent.</param>
        public NavsTabComponent(By selector,
            IWebDriver driver,
            NavsTabComponentConfiguration configuration,
            T parent)
            : base(selector, driver, parent)
        {
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> TabHeaderNameElements => WrappedDriver
            .FindElements(configuration.TabHeaderNamesSelector);

        private IReadOnlyCollection<IWebElement> TabHeaderElements => WrappedElement
            .FindElements(configuration.TabHeaderElementsSelector);

        private IWebElement ActiveTabContentElement => WrappedElement
            .FindElement(configuration.ActiveTabContentElementSelector);

        private IWebElement ActiveTabHeaderElement => WrappedElement
            .FindElement(configuration.ActiveTabHeaderElementSelector);

        private IReadOnlyCollection<IWebElement> TabContentElements => WrappedElement
            .FindElements(configuration.TabContentElementsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the tab names.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<string> GetTabNames()
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
        public virtual IReadOnlyCollection<IWebElement> GetTabHeaderElements()
        {
            return TabHeaderElements;
        }

        /// <summary>
        /// Gets the active tab body.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual IWebElement GetActiveTabBody()
        {
            return ActiveTabContentElement;
        }

        /// <summary>
        /// Gets the name of the active tab.
        /// </summary>
        /// <returns></returns>
        public virtual string GetActiveTabName()
        {
            return WrappedElement
                .FindElement(configuration.ActiveTabHeaderNameSelector)
                .TextHelper()
                .InnerText;
        }

        /// <summary>
        /// Determines whether the specified tab exists.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public virtual bool HasTab(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (String.IsNullOrEmpty(tabName))
                throw new ArgumentNullException(nameof(tabName));

            return GetTabNames().Any(tn => String.Equals(
                tn,
                tabName,
                stringComparison));
        }

        /// <summary>
        /// Gets the tab body of a tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public IWebElement GetTabBody(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var headerElement = GetTabHeaderElements()
                .FirstOrDefault(
                    e => String.Equals(
                        tabName,
                        e.TextHelper().InnerText,
                        stringComparison));

            if (headerElement == null)
                throw new NoSuchElementException();

            var sel = headerElement.GetAttribute("href");
            var bodyElement = WrappedElement.FindElement(By.CssSelector(sel));

            if (bodyElement == null)
                throw new NoSuchElementException();

            return bodyElement;
        }

        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="tabName">Name of the tab.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <exception cref="ArgumentNullException">tabName</exception>
        public void SelectTab(string tabName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (String.IsNullOrEmpty(tabName))
                throw new ArgumentNullException(nameof(tabName));

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

            // Need to create a waiter for the event.
            var waiter = tab.GetEventWaiter("shown.bs.tab");

            // Now perform the action.
            tab.Click();

            // And wait for the event to be emitted.
            waiter.Wait(TimeSpan.FromMilliseconds(500));
        }

        #endregion
    }
}
