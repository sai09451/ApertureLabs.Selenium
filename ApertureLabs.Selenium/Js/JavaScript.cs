using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// Base class for writing javascript.
    /// </summary>
    public class JavaScript
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScript"/> class.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <exception cref="ArgumentNullException">script</exception>
        public JavaScript(string script)
        {
            if (String.IsNullOrEmpty(script))
                throw new ArgumentNullException(nameof(script));

            Script = script;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScript"/> class.
        /// </summary>
        public JavaScript()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the arguments passed into the script when executing
        /// it.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public virtual object[] Arguments { get; set; }

        /// <summary>
        /// The script to be executed.
        /// </summary>
        public virtual string Script { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this script is
        /// asynchronous. This changes how Execute() operates, if true then the
        /// script will be executed using ExecuteAsyncScript(...), if false
        /// then ExecuteScript(...).
        /// </summary>
        public virtual bool IsAsync { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the script.
        /// </summary>
        public virtual void Format()
        {
            Script = EscapeScript(Script);
            Script = TrimWhitespace(Script);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="executor">The executor.</param>
        public virtual void Execute(IJavaScriptExecutor executor)
        {
            var result = IsAsync
                ? executor.ExecuteAsyncScript(Script, Arguments)
                : executor.ExecuteScript(Script, Arguments);
        }

        /// <summary>
        /// Executes the with result. Accepts the following type arguments:
        /// <c>int</c>, <c>double</c>, <c>long</c>, <c>decimal</c>,
        /// <c>string</c>, <c>bool</c>, and <c>IWebElement</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">The executor.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// Thrown if a typeargument is passed in that's not supported.
        /// </exception>
        public virtual T ExecuteWithResult<T>(IJavaScriptExecutor executor)
        {
            var passedInType = typeof(T);

            var specialParsingTypes = new[]
            {
                typeof(int),
                typeof(double),
                typeof(long),
                typeof(decimal),
            };

            var otherAcceptedTypes = new[]
            {
                typeof(IWebElement),
                typeof(string),
                typeof(bool)
            };

            if (!otherAcceptedTypes.Contains(passedInType)
                || !specialParsingTypes.Contains(passedInType))
            {
                throw new NotImplementedException("WebDriver doesn't " +
                    "support that type yet.");
            }

            var result = IsAsync
                ? executor.ExecuteAsyncScript(Script, Arguments)
                : executor.ExecuteScript(Script, Arguments);

            if (specialParsingTypes.Contains(passedInType))
            {
                // Since all of the int/decimal/long/number types of a static
                // Parse method, we'll use that to get the result.
                var parsedResult = passedInType.GetMethod("Parse").Invoke(
                    null,
                    new object[] { result.ToString() });

                return (T)parsedResult;
            }
            else
            {
                return (T)result;
            }
        }

        /// <summary>
        /// Tries to execute with result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="executor">The executor.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public virtual bool TryExecuteWithResult<T>(
            IJavaScriptExecutor executor,
            out T result,
            out Exception exception)
        {
            exception = null;
            result = default;

            try
            {
                result = ExecuteWithResult<T>(executor);
                return true;
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
        }

        /// <summary>
        /// Tries the execute.
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public virtual bool TryExecute(IJavaScriptExecutor executor,
            out Exception exception)
        {
            exception = null;

            try
            {
                Execute(executor);
                return true;
            }
            catch (Exception exc)
            {
                exception = exc;
                return false;
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Script;
        }

        /// <summary>
        /// Escapes the script. Replaces all '{' and '}' that aren't followed
        /// by a number with '{{' or '}}'. Also calls TrimWhitespace on the
        /// script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public static string EscapeScript(string script)
        {
            var dontReplace = Regex.Matches(script, @"(\{\d+\}|\{{2}|\}{2})");
            var leftBraceMatches = Regex.Matches(script, @"\{(?!\d+\})");
            var rightBraceMatches = Regex.Matches(script, @"(?<!\{\d+)\}");
            var allMatches = new List<Match>();
            var indeciesToReplace = new List<int>();
            var ignore = new List<Tuple<int, int>>();

            foreach (Match ignoreMatch in dontReplace)
            {
                var start = ignoreMatch.Index;
                var end = start + ignoreMatch.Length;

                ignore.Add(Tuple.Create(start, end));
            }

            allMatches.AddRange(leftBraceMatches.Cast<Match>());
            allMatches.AddRange(rightBraceMatches.Cast<Match>());
            allMatches = allMatches.OrderBy(i => i.Index).ToList();

            foreach (var match in allMatches)
            {
                // Check if the match is in the range of any of the ignored
                // items.
                var shouldIgnore = ignore.Any(
                    i => match.Index >= i.Item1 && match.Index <= i.Item2);

                if (shouldIgnore)
                    continue;

                // Replace the match.
                indeciesToReplace.Add(match.Index);
            }

            for (var i = 0; i < indeciesToReplace.Count; i++)
            {
                var index = indeciesToReplace[i];
                script = script.Insert(index,
                    script[index].Equals('{') ? "{" : "}");

                // Updated the other indicies to account for the offset.
                for (var ii = i; ii < indeciesToReplace.Count; ii++)
                    indeciesToReplace[ii]++;
            }

            return TrimWhitespace(script);
        }

        /// <summary>
        /// Removes all line breaks in a script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public static string TrimWhitespace(string script)
        {
            var regex = new Regex("[ ]{2,}", RegexOptions.None);

            // Remove all extra spaces.
            script = Regex.Replace(script, "[ ]{2,}", " ");

            // Remove all newlines and tabs (handles ill-formed newlines).
            script = Regex.Replace(script, @"[\t|\n|\r|\r\n|\n]", "");

            return script;
        }

        /// <summary>
        /// Removes the comments.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public static string RemoveComments(string script)
        {
            // Remove all // comments.
            script = Regex.Replace(script, @"\/\/.*$", "");

            // Remove all /**/ comments.
            script = Regex.Replace(script, @"\/\*\*.*?\*\/", "");

            return script;
        }

        #region Implicit Conversions

        /// <summary>
        /// Performs an explicit conversion from <see cref="JavaScript"/> to
        /// <see cref="System.String"/>.
        /// </summary>
        /// <param name="js">The js.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator string(JavaScript js)
        {
            return js.Script;
        }

        /// <summary>
        /// Implements the operator +. Returns a new JavaScript object with
        /// the Script property being set to the result of (a + b).
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static JavaScript operator+(JavaScript a, JavaScript b)
        {
            var scripts = a.Script + b.Script;

            return new JavaScript(scripts);
        }

        #endregion

        #endregion
    }
}
