using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// Provides a way to provide type-safe arguments to a script and return a
    /// type safe value.
    /// </summary>
    public class TypedJavaScriptExecutor
    {
        #region Fields

        private readonly IJavaScriptExecutor javaScriptExecutor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedJavaScriptExecutor"/> class.
        /// </summary>
        /// <param name="javaScriptExecutor">The java script executor.</param>
        public TypedJavaScriptExecutor(IJavaScriptExecutor javaScriptExecutor)
        {
            this.javaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes the JavaScript.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public virtual JavaScriptValue ExecuteJavaScript(string script,
            params JavaScriptValue[] arguments)
        {
            var convertedArgs = arguments
                .Select(a => a.GetArgument())
                .ToArray();
            var result = javaScriptExecutor.ExecuteScript(
                script,
                convertedArgs);

            return new JavaScriptValue(result);
        }

        /// <summary>
        /// Executes the asynchronous script.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public virtual JavaScriptValue ExecuteAsyncScript(string script,
            params JavaScriptValue[] arguments)
        {
            var convertedArgs = arguments
                .Select(a => a.GetArgument())
                .ToArray();
            var result = javaScriptExecutor.ExecuteAsyncScript(
                script,
                convertedArgs);

            return new JavaScriptValue(result);
        }

        /// <summary>
        /// Executes the JavaScript.
        /// </summary>
        /// <param name="javaScript">The JavaScript.</param>
        /// <returns></returns>
        public virtual JavaScriptValue ExecuteJavaScript(JavaScript javaScript)
        {
            return javaScript.Execute(javaScriptExecutor);
        }

        #endregion
    }
}
