using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration
{
    /// <summary>
    /// Contains general info on a razor page.
    /// </summary>
    public class RazorPageInfo
    {
        public RazorPageInfo()
        {
            ImportantElementSelectors = new Dictionary<string, string>();
            PartialViews = new Dictionary<string, bool>();
        }

        /// <summary>
        /// The relative path to the file from the project root.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// The path to the layout page referenced by this page. Can be null.
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// A list of important selectors and the element tag name they're
        /// applied to.
        /// </summary>
        public IDictionary<string, string> ImportantElementSelectors { get; }

        /// <summary>
        /// Partial views names and if they're in a for loop (IE: Should be
        /// IEnumerable).
        /// </summary>
        public IDictionary<string, bool> PartialViews { get; }

        /// <summary>
        /// Contains the css selector to the nested components on the page.
        /// </summary>
        public IDictionary<string, RazorComponent> NestedComponents { get; }

        /// <summary>
        /// Whether the razor page is a template page object, static page
        /// object, parameter page object, or a page component.
        /// </summary>
        public string TypeOfRazorPage { get; set; }
    }

    public class RazorComponent
    {
        public bool Multiple { get; set; }
    }
}
