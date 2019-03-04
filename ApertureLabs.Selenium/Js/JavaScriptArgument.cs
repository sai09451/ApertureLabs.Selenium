using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// Safely converts the argument into a type safe version for passing into
    /// scripts.
    /// </summary>
    public class JavaScriptArgument
    {
        #region Fields

        private readonly object argument;

        #endregion

        #region Constructor(s)

        private JavaScriptArgument(object argument)
        {
            this.argument = argument;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="argument">The element.</param>
        public JavaScriptArgument(IWebElement argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptArgument(IEnumerable<IWebElement> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public JavaScriptArgument(string argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptArgument(IEnumerable<string> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public JavaScriptArgument(long argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptArgument(IEnumerable<long> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        public JavaScriptArgument(bool argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptArgument(IEnumerable<bool> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptArgument"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptArgument(IEnumerable<JavaScriptArgument> arguments)
        {
            var convertedArguments = arguments
                .Select(argument => argument.GetArgument())
                .ToList();

            argument = convertedArguments;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the argument.
        /// </summary>
        /// <returns></returns>
        public object GetArgument()
        {
            return argument;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="RemoteWebElement"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(RemoteWebElement argument)
        {
            var el = argument.UnWrapEventFiringWebElement();

            return new JavaScriptArgument(el);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RemoteWebElement[]"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(RemoteWebElement[] arguments)
        {
            return new JavaScriptArgument(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(string argument)
        {
            return new JavaScriptArgument(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String[]"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(string[] arguments)
        {
            return new JavaScriptArgument(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int64"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(long argument)
        {
            return new JavaScriptArgument(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int64[]"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(long[] arguments)
        {
            return new JavaScriptArgument(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(bool argument)
        {
            return new JavaScriptArgument(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean[]"/> to <see cref="JavaScriptArgument"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptArgument(bool[] arguments)
        {
            return new JavaScriptArgument(arguments);
        }

        #endregion
    }
}
