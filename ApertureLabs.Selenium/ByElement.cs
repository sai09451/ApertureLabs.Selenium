using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Will always ONLY return the stored element.
    /// </summary>
    /// <seealso cref="OpenQA.Selenium.By" />
    public class ByElement : By
    {
        #region Fields

        private readonly IWebElement element;

        #endregion

        #region Constructor

        internal ByElement(IWebElement element)
        {
            this.element = element;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the stored element.
        /// </summary>
        /// <param name="context">An <see cref="T:OpenQA.Selenium.ISearchContext" /> object to use to search for the elements.</param>
        /// <returns>
        /// The first matching <see cref="T:OpenQA.Selenium.IWebElement" /> on the current context.
        /// </returns>
        public override IWebElement FindElement(ISearchContext context)
        {
            return element;
        }

        /// <summary>
        /// Returns the stored element.
        /// </summary>
        /// <param name="context">An <see cref="T:OpenQA.Selenium.ISearchContext" /> object to use to search for the elements.</param>
        /// <returns>
        /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of all <see cref="T:OpenQA.Selenium.IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.
        /// </returns>
        public override ReadOnlyCollection<IWebElement> FindElements(
            ISearchContext context)
        {
            return new ReadOnlyCollection<IWebElement>(new[] { element });
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static By FromElement(IWebElement element)
        {
            return new ByElement(element);
        }

        #endregion
    }
}
