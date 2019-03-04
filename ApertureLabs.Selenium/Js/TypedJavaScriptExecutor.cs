using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.Js
{
    public class TypedJavaScriptExecutor
    {
        #region Fields

        private readonly IJavaScriptExecutor javaScriptExecutor;

        #endregion

        #region Constructor

        public TypedJavaScriptExecutor(IJavaScriptExecutor javaScriptExecutor)
        {
            this.javaScriptExecutor = javaScriptExecutor;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public string ExecuteJavaScript(string script,
            params JavaScriptArgument[] arguments)
        {
            var convertedArgs = arguments.Select(a => a.GetArgument());
            var result = javaScriptExecutor.ExecuteScript(
                script,
                convertedArgs);

            if (result is string castedResult)
                return castedResult;
            else
                throw new InvalidCastException();
        }

        #endregion
    }
}
