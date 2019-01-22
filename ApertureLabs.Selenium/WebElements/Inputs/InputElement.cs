using System;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// Wrapper around an IWebElement to provide extra functionality for input
    /// elements.
    /// </summary>
    public class InputElement : BaseElement
    {
        #region Constructor

        /// <summary>
        /// Checks that the element has the correct tagname.
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="InvalidCastException">
        /// Thrown when the elements tagname isn't input.
        /// </exception>
        public InputElement(IWebElement element)
            : base(element)
        {
            if (!String.Equals(element.TagName,
                "input",
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnexpectedTagNameException("The element wasn't an input element.");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the type of the input element.
        /// </summary>
        public string Type
        {
            get
            {
                var type = GetAttribute("type");

                if (String.IsNullOrEmpty(type))
                    type = "text";

                return type;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the value and tries to cast it to the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetValue<T>() where T:IConvertible
        {
            var value = WrappedElement.GetProperty("value");

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Returns and converts the value by passing in the conversion
        /// function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        /// <returns></returns>
        public virtual T GetValue<T>(Func<string, T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            var value = WrappedElement.GetProperty("value");

            try
            {
                return converter(value);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Sets the 'value' of the input element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public virtual void SetValue<T>(T value) where T:IConvertible
        {
            var asString = (string)Convert.ChangeType(value, typeof(string));
            WrappedElement.Clear();

            if (!String.IsNullOrEmpty(asString))
                WrappedElement.SendKeys(asString);
        }

        #endregion
    }
}