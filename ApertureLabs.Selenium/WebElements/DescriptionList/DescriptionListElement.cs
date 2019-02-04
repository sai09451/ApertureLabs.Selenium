using System;
using System.Collections.Generic;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.WebElements.DescriptionList
{
    /// <summary>
    /// DescriptionListElement.
    /// </summary>
    /// <seealso cref="OpenQA.Selenium.IWebElement" />
    public class DescriptionListElement : BaseElement
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionListElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <exception cref="UnexpectedTagNameException">dl</exception>
        public DescriptionListElement(IWebElement element)
            : base(element)
        {
            // Verify the tagname is correct.
            var validTagName = String.Equals(
                element.TagName,
                "dl",
                StringComparison.OrdinalIgnoreCase);

            if (!validTagName)
                throw new UnexpectedTagNameException("dl", element.TagName);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a list of all dt-elements with their corresponding
        /// dd-elements.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<(IWebElement term, IWebElement detail)> GetDescriptions()
        {
            var results = new List<(IWebElement term, IWebElement detail)>();
            var children = WrappedElement.Children();
            //var currentGroup = Tuple.Create<IWebElement, IWebElement>(null, null);

            for (var i = 0; i < children.Count; i++)
            {
                var termEl = children[i];
                var detailsEl = default(IWebElement);

                // Ignore if the element isn't dt.
                if (!HasTagName("dt", termEl))
                    continue;

                // Locate the next dd element.
                i++;
                for ( ; i < children.Count; i++)
                {
                    var el = children[i];

                    if (!HasTagName("dd", el))
                        continue;

                    detailsEl = el;
                    results.Add((termEl, detailsEl));
                    break;
                }
            }

            return results;
        }

        private bool HasTagName(string tagName, IWebElement element)
        {
            return String.Equals(
                tagName,
                element.TagName,
                StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
