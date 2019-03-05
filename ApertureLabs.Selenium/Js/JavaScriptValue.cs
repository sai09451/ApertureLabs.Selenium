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
    public class JavaScriptValue
    {
        #region Fields

        private readonly object argument;
        private readonly JavaScriptType javaScriptType;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="argument">
        /// The argument. An exception will be thrown if it the type is
        /// supported
        /// </param>
        /// <param name="javaScriptType">
        /// Type of the JavaScript value will be checked against the actual
        /// type of argument and will throw an exception if they don't match.
        /// </param>
        /// <exception cref="InvalidCastException"></exception>
        public JavaScriptValue(object argument,
            JavaScriptType? javaScriptType = null)
        {
            this.argument = argument;
            var typeOfArg = GetJavaScripType(argument);

            if (javaScriptType != null && typeOfArg != javaScriptType.Value)
            {
                throw new InvalidCastException($"Failed to cast the argument " +
                    $"({argument.GetType().Name}) to the provided type.");
            }

            this.javaScriptType = typeOfArg;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="argument">The element.</param>
        public JavaScriptValue(IWebElement argument)
            : this(
                  (object)argument.UnWrapEventFiringWebElement(),
                  JavaScriptType.WebElement)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptValue(IEnumerable<IWebElement> arguments)
            : this(
                  (object)arguments.Select(e => e.UnWrapEventFiringWebElement()).ToArray(),
                  JavaScriptType.WebElementArray)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public JavaScriptValue(string argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptValue(IEnumerable<string> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="argument">The argument.</param>
        public JavaScriptValue(long argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptValue(IEnumerable<long> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        public JavaScriptValue(bool argument)
            : this((object)argument)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptValue(IEnumerable<bool> arguments)
            : this((object)arguments)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JavaScriptValue"/> class.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        public JavaScriptValue(IEnumerable<JavaScriptValue> arguments)
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

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        /// <returns></returns>
        public JavaScriptType GetArgumentType()
        {
            return javaScriptType;
        }

        /// <summary>
        /// Determines whether the argument is null.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNull()
        {
            return argument == null;
        }

        /// <summary>
        /// Converts the argument to a bool.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public bool ToBool()
        {
            return (bool)argument;
        }

        /// <summary>
        /// Converts the argument to an IEnumerable{bool}.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IReadOnlyCollection<bool> ToBoolArray()
        {
            return (argument as IReadOnlyCollection<object>)
                .Cast<bool>()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Converts the argument to a number.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public long ToNumber()
        {
            return (long)argument;
        }

        /// <summary>
        /// Converts the argument to an IEnumerable{long}.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IReadOnlyCollection<long> ToNumberArray()
        {
            return (argument as IReadOnlyCollection<object>)
                .Cast<long>()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Converts the argument to a string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        /// <exception cref="InvalidCastException"></exception>
        public override string ToString()
        {
            return (string)argument;
        }

        /// <summary>
        /// Converts the argument to a stringarray.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IReadOnlyCollection<string> ToStringArray()
        {
            return (argument as IReadOnlyCollection<object>)
                .Cast<string>()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Converts the argument to a webelement.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IWebElement ToWebElement()
        {
            return (IWebElement)argument;
        }

        /// <summary>
        /// Converts the argument to a webelementarray.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidCastException"></exception>
        public IReadOnlyCollection<IWebElement> ToWebElementArray()
        {
            return (argument as IReadOnlyCollection<object>)
                .Cast<IWebElement>()
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Converts the argument to a multi-type array.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<JavaScriptValue> ToMultiTypeArray()
        {
            return (argument as IReadOnlyCollection<object>)
                .Select(a => new JavaScriptValue(a))
                .ToList()
                .AsReadOnly();
        }

        private JavaScriptType GetJavaScripType(object obj)
        {
            if (obj == null)
                return JavaScriptType.Null;

            switch (argument)
            {
                case bool boolArg:
                    return JavaScriptType.Boolean;
                case string strArg:
                    return JavaScriptType.String;
                case long numberArg:
                    return JavaScriptType.Number;
                case IWebElement elementArg:
                    return JavaScriptType.WebElement;

                // All arrays are returned as IReadOnlyCollection<object>.
                case IReadOnlyCollection<object> multiTypeArrArg:
                    var firstItem = multiTypeArrArg.First();
                    var firstItemType = firstItem?.GetType();

                    // Check if array contains a single type or multiple types.
                    if (multiTypeArrArg.All(i => i.GetType() == firstItemType))
                    {
                        // If it's an array of nulls treat it as a multi-type.
                        if (firstItem == null)
                            return JavaScriptType.MultiTypeArray;

                        switch (firstItem)
                        {
                            case long numberArg:
                                return JavaScriptType.NumberArray;
                            case bool boolArg:
                                return JavaScriptType.BooleanArray;
                            case string stringArg:
                                return JavaScriptType.StringArray;
                            case IWebElement elementArg:
                                return JavaScriptType.WebElementArray;
                        }

                        break;
                    }
                    else
                    {
                        return JavaScriptType.MultiTypeArray;
                    }
            }

            throw new NotImplementedException($"The type " +
                $"({obj.GetType().Name}) isn't supported.");
        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs an implicit conversion from <see cref="RemoteWebElement"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(RemoteWebElement argument)
        {
            var el = argument.UnWrapEventFiringWebElement();

            return new JavaScriptValue(el);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RemoteWebElement[]"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(RemoteWebElement[] arguments)
        {
            return new JavaScriptValue(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(string argument)
        {
            return new JavaScriptValue(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String[]"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(string[] arguments)
        {
            return new JavaScriptValue(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int64"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(long argument)
        {
            return new JavaScriptValue(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int64[]"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(long[] arguments)
        {
            return new JavaScriptValue(arguments);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="argument">if set to <c>true</c> [argument].</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(bool argument)
        {
            return new JavaScriptValue(argument);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean[]"/> to <see cref="JavaScriptValue"/>.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator JavaScriptValue(bool[] arguments)
        {
            return new JavaScriptValue(arguments);
        }

        #endregion
    }
}
