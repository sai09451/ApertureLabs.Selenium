using OpenQA.Selenium;
using System;
using System.Linq;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// A version of by that selects element(s) that match all of the
    /// selectors.
    /// </summary>
    public class ByAll : By
    {
        #region Fields

        private readonly By[] selectors;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ByAll"/> class.
        /// </summary>
        /// <param name="selectors">The selectors.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if selectors is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if selectors is empty.
        /// </exception>
        public ByAll(params By[] selectors)
        {
            if (selectors == null)
                throw new ArgumentNullException(nameof(selectors));
            else if (!selectors.Any())
                throw new ArgumentOutOfRangeException(nameof(selectors));

            this.selectors = selectors;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the first element matching the criteria.
        /// </summary>
        /// <param name="context">An <see cref="T:OpenQA.Selenium.ISearchContext" /> object to use to search for the elements.</param>
        /// <returns>
        /// The first matching <see cref="T:OpenQA.Selenium.IWebElement" /> on the current context.
        /// </returns>
        public override IWebElement FindElement(ISearchContext context)
        {
            var elements = selectors.Select(s => context.FindElement(s));
            var firstElement = elements.First();
            var matchesAll = elements.All(e => e.Equals(firstElement));

            if (matchesAll)
                return firstElement;
            else
                throw new NoSuchElementException(ToString());
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Join(", ", selectors.Select(s => s.ToString()));
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        /// <param name="selectors">The selectors.</param>
        /// <returns></returns>
        public static By FindByAll(params By[] selectors)
        {
            return new ByAll(selectors) as By;
        }

        #endregion
    }
}
