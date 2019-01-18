using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Similar to the normal <code>OpenQA.Selenium.By</code> class but will
    /// always search from the passed in searchContext.
    /// </summary>
    public class ScopedBy : By
    {
        private readonly ISearchContext searchContext;
        private readonly ICollection<By> subSelectors;
        private readonly By contextSelector;

        /// <summary>
        /// Ctor. To create an instance use the static method FromScoped(...).
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="subSelectors"></param>
        internal ScopedBy(ISearchContext searchContext,
            ICollection<By> subSelectors)
        {
            this.Description = "ApertureLabs.Selenium.ScopedBy";
            this.searchContext = searchContext;
            this.subSelectors = subSelectors;
        }

        /// <summary>
        /// Ctor. Instead fo passing in an instance of ISearchContext, pass in a
        /// selector that will fetch the ISearchContext and then the subSelector
        /// will search for the child elements.
        /// </summary>
        /// <param name="contextSelector"></param>
        /// <param name="subSelectors"></param>
        internal ScopedBy(By contextSelector,
            ICollection<By> subSelectors)
        {
            this.contextSelector = contextSelector;
            this.subSelectors = subSelectors;
        }

        /// <inheritdoc/>
        public override IWebElement FindElement(ISearchContext context)
        {
            var searchContext = this.searchContext
                ?? context.FindElement(contextSelector);

            var element = default(IWebElement);

            foreach (var selector in subSelectors)
            {
                var _context = element ?? searchContext;
                element = _context.FindElement(selector);
            }

            return element;
        }

        /// <inheritdoc/>
        public override ReadOnlyCollection<IWebElement> FindElements(
            ISearchContext context)
        {
            var searchContext = this.searchContext
                ?? context.FindElement(contextSelector);

            var previousContexts = new List<ISearchContext> { searchContext };

            for (var i = 0; i < subSelectors.Count; i++)
            {
                var selector = subSelectors.ElementAt(i);
                var newContexts = new List<IWebElement>();

                foreach (var c in previousContexts)
                {
                    var els = c.FindElements(selector)
                        .Where(e => !newContexts.Contains(e));

                    newContexts.AddRange(els);
                }

                previousContexts = newContexts.Cast<ISearchContext>().ToList();

                // Exit loop early if no child elements matching the selector
                // were found.
                if (!previousContexts.Any())
                {
                    break;
                }
            }

            var elements = previousContexts.Cast<IWebElement>()
                .ToList()
                .AsReadOnly();

            return elements;
        }

        /// <summary>
        /// Creates a 'scoped' selector which will always use the passed in
        /// search context instead of what the current context is for the
        /// driver.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="subSelectors"></param>
        /// <returns></returns>
        public static By FromScope(
            ISearchContext searchContext,
            ICollection<By> subSelectors)
        {
            if (searchContext == null)
            {
                throw new ArgumentNullException(nameof(searchContext));
            }
            else if (subSelectors == null)
            {
                throw new ArgumentNullException(nameof(subSelectors));
            }

            return new ScopedBy(searchContext, subSelectors) as OpenQA.Selenium.By;
        }

        /// <summary>
        /// Creates a 'scoped' selector which will first find all elements
        /// that match the <code>searchContext</code> selector and then keep
        /// applying the <code>subSelectors</code> to those elements.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="subSelectors"></param>
        /// <returns></returns>
        public static By FromScope(
            By searchContext,
            ICollection<By> subSelectors)
        {
            if (searchContext == null)
            {
                throw new ArgumentNullException(nameof(searchContext));
            }
            else if (searchContext == null)
            {
                throw new ArgumentNullException(nameof(subSelectors));
            }

            return new ScopedBy(searchContext, subSelectors);
        }

        /// <summary>
        /// Creates a 'scoped' selector which will first find all elements
        /// that match the <code>searchContext</code> selector and then keep
        /// applying the <code>subSelectors</code> to those elements.
        /// </summary>
        /// <param name="searchContext"></param>
        /// <param name="subSelectors"></param>
        /// <returns></returns>
        public static By FromScope(
            By searchContext,
            params By[] subSelectors)
        {
            return FromScope(searchContext, subSelectors.ToList());
        }
    }
}
