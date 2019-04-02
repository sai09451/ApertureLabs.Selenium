using EnvDTE;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.CodeGeneration.Extensions
{
    /// <summary>
    /// Extensions for <see cref="CodeElements"/>.
    /// </summary>
    public static class CodeElementsExtensions
    {
        /// <summary>
        /// Recurses thru all code elements.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <returns></returns>
        public static IEnumerable<CodeElement> Recurse(
            this CodeElements codeElements)
        {
            foreach (CodeElement codeElement in codeElements)
            {
                yield return codeElement;

                foreach (CodeElement subCodeElement in Recurse(codeElement.Children))
                {
                    yield return subCodeElement;
                }
            }
        }

        /// <summary>
        /// Searches for all <see cref="CodeElement"/>s that match the query.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static CodeElement SearchFor(this CodeElements codeElements,
            Func<CodeElement, bool> query)
        {
            var result = default(CodeElement);

            // Check all direct descendent elements.
            foreach (CodeElement codeElement in codeElements)
            {
                if (query(codeElement))
                {
                    result = codeElement;
                    break;
                }

                // Check all nested elements.
                var childMatch = SearchFor(codeElement.Children, query);

                if (childMatch != null)
                {
                    result = childMatch;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Searches for all <see cref="CodeElement"/>s that match the query.
        /// </summary>
        /// <param name="codeElements">The code elements.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static IEnumerable<CodeElement> SearchForAll(
            this CodeElements codeElements,
            Func<CodeElement, bool> query)
        {
            foreach (var codeElement in Recurse(codeElements))
            {
                if (query(codeElement))
                    yield return codeElement;
            }
        }
    }
}
