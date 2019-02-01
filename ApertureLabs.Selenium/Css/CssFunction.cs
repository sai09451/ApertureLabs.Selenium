using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Represents a css function.
    /// </summary>
    public class CssFunction : CssValue
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CssFunction"/> class.
        /// </summary>
        /// <param name="value"></param>
        public CssFunction(string value) : base(value)
        {
            var results = new List<CssValue>();
            var argumentsRegex = new Regex(@".*\((.*)\)");
            var match = argumentsRegex.Match(Value).Groups[1].Value;

            // Split on every comma/whitespace.
            var args = match.Split(',', ' ')
                .Where(s => !String.IsNullOrEmpty(s))
                .ToList();

            foreach (var arg in args)
            {
                var _value = new CssValue(arg);
                results.Add(_value);
            }

            Arguments = results;

            match = new Regex(@"(.*)\(.*\)").Match(Value).Groups[1].Value;
            FunctionName = match;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public IReadOnlyList<CssValue> Arguments { get; private set; }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public string FunctionName { get; private set; }

        #endregion
    }
}
