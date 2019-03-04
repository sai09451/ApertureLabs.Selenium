using System;
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
        public virtual JavaScriptValue[] Arguments { get; set; }

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
            Script = RemoveComments(Script);
            Script = Clean(Script);
        }

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="executor">The executor.</param>
        public virtual JavaScriptValue Execute(IJavaScriptExecutor executor)
        {
            var result = IsAsync
                ? executor.ExecuteAsyncScript(Script, Arguments)
                : executor.ExecuteScript(Script, Arguments);

            return new JavaScriptValue(result);
        }

        /// <summary>
        /// Tries the execute.
        /// </summary>
        /// <param name="executor">The executor.</param>
        /// <param name="result">The result.</param>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public virtual bool TryExecute(IJavaScriptExecutor executor,
            out JavaScriptValue result,
            out Exception exception)
        {
            exception = null;
            result = null;

            try
            {
                result = new JavaScriptValue(Execute(executor));
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
        /// Removes all line breaks, tabs, and comments in a script. WARNING:
        /// Make sure all comments are removed prior to calling this.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <returns></returns>
        public static string Clean(string script)
        {
            // Remove all extra spaces.
            script = Regex.Replace(script, "[ ]{2,}", " ");

            // Remove all newlines and tabs (handles ill-formed newlines).
            script = Regex.Replace(script, @"[\t\n\r]", "");

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
            script = Regex.Replace(
                script,
                @"\/\/.*$",
                "",
                RegexOptions.Multiline);
            // Remove all /**/ comments.
            script = Regex.Replace(
                script,
                @"\/\*\*.*?\*\/",
                "",
                RegexOptions.Singleline);

            return script;
        }

        /// <summary>
        /// Appends the two scripts together and returns a new
        /// <see cref="JavaScript"/> object.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static JavaScript Add(JavaScript a, JavaScript b)
        {
            var scripts = a.Script + b.Script;

            return new JavaScript(scripts);
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
