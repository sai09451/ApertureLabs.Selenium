﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Contains extensions for ISearchContext objects.
    /// </summary>
    public static class ISearchContextExtensions
    {
        /// <summary>
        /// Creates a unique CSS selector for an element.
        /// </summary>
        /// <param name="searchContext">The search context.</param>
        /// <param name="element">The element.</param>
        /// <param name="optimize">
        /// Whether or not to optimize the selector. This may be time
        /// consuming.
        /// </param>
        /// <returns></returns>
        public static By GetCssSelector(
            this ISearchContext searchContext,
            IWebElement element,
            bool optimize = false)
        {
            var foundUniqueSelector = false;
            var reachedDocumentParent = false;
            var selectorParts = new List<string>();
            var currentEl = element;

            #region Helper local functions

            By GetFullSelector(List<string> cssParts = null)
            {
                return By.CssSelector(String.Join(" ", cssParts ?? selectorParts));
            }

            bool IsSelectorValid(By selector = null)
            {
                selector = selector ?? GetFullSelector();
                var results = searchContext.FindElements(selector);

                // Check if the element matches the one passed it.
                return results.Count == 1
                    ? results[0].Equals(element)
                    : false;
            }

            bool IsSelectorPartNeeded(int index)
            {
                var copyOfSelector = new List<string>(selectorParts);
                copyOfSelector.RemoveAt(index);
                var selector = GetFullSelector(copyOfSelector);

                return IsSelectorValid(selector);
            }

            #endregion

            // Keep iterating until we've reached the parent document or we've
            // found a unique selector.
            while (!foundUniqueSelector && !reachedDocumentParent)
            {
                bool shouldCheckSelector = false;

                if (TryGetAttribute(currentEl, "id", out var idAttr))
                {
                    // Use the id.
                    var formattedId = idAttr.Contains(" ")
                        ? $"*[id='{idAttr}']"
                        : $"#{idAttr}";

                    shouldCheckSelector = true;
                    selectorParts.Insert(0, formattedId);
                }
                else if (TryGetAttribute(currentEl, "classes", out var classAttr))
                {
                    // Use the classes.
                    var formattedClasses = classAttr
                        .Insert(0, ".")
                        .Replace(" ", ".");

                    shouldCheckSelector = true;
                    selectorParts.Insert(0, formattedClasses);
                }
                else if (TryGetAttribute(currentEl, "role", out var roles))
                {
                    // Use the role attribute.
                    shouldCheckSelector = true;
                    var formattedRole = $"*[role='{roles}']";
                }
                else
                {
                    // Use the tagname[ + index if greather than 0].
                    var selector = currentEl.TagName;
                    var useIndex = currentEl
                        ?.GetParentElement()
                        ?.Children()
                        .Count > 1;

                    if (useIndex)
                    {
                        var index = currentEl.GetIndexRelativeToSiblings();
                        selector += $":nth-child({index + 1})";
                    }

                    selectorParts.Insert(0, selector);
                }

                // Set currentEl to the parent element.
                currentEl = currentEl.GetParentElement();

                // Check if we've reached the document parent element.
                reachedDocumentParent = currentEl == null;

                if (shouldCheckSelector || reachedDocumentParent)
                {
                    var results = searchContext
                        .FindElements(GetFullSelector())
                        .Count;

                    if (IsSelectorValid())
                    {
                        // Found a good match.
                        foundUniqueSelector = true;
                    }
                }
            }

            if (optimize)
            {
                var copyOfSelectorParts = new List<string>(selectorParts);

                for (var i = 0; i < selectorParts.Count; i++)
                {
                    if (!IsSelectorPartNeeded(i))
                    {
                        selectorParts.RemoveAt(i);
                        i--;
                    }
                }
            }

            // Throw exception if no selector could be created.
            if (!IsSelectorValid())
            {
                throw new InvalidSelectorException("Failed to create a " +
                    "unique selector for this element.");
            }

            return GetFullSelector();
        }

        private static bool TryGetAttribute(IWebElement element,
            string attribute,
            out string value)
        {
            value = default;

            try
            {
                value = element.GetAttribute(attribute);
            }
            catch
            { }

            return !String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Tries the get attribute. Will check if the value is null BEFORE
        /// executing the expression.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static bool TryGetAttribute(IWebElement element,
            string attribute,
            Func<string, bool> predicate,
            out string value)
        {
            value = element.GetAttribute(attribute);

            return String.IsNullOrEmpty(value) ? false : predicate(value);

        }
    }
}
