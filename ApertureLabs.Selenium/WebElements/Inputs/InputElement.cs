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
    public class InputElement : IWebElement
    {
        #region Fields

        private readonly IWebElement Element;

        #endregion

        #region Constructor

        /// <summary>
        /// Checks that the element has the correct tagname.
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="InvalidCastException">
        /// Thrown when the elements tagname isn't input.
        /// </exception>
        public InputElement(IWebElement element)
        {
            if (!String.Equals(element.TagName,
                "input",
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnexpectedTagNameException("The element wasn't an input element.");
            }

            Element = element;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string TagName => Element.TagName;

        /// <inheritdoc/>
        public string Text => Element.Text;

        /// <inheritdoc/>
        public bool Enabled => Element.Enabled;

        /// <inheritdoc/>
        public bool Selected => Element.Selected;

        /// <inheritdoc/>
        public Point Location => Element.Location;

        /// <inheritdoc/>
        public Size Size => Element.Size;

        /// <inheritdoc/>
        public bool Displayed => Element.Displayed;

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

        /// <inheritdoc/>
        public void Clear()
        {
            Element.Clear();
        }

        /// <inheritdoc/>
        public void Click()
        {
            Element.Click();
        }

        /// <inheritdoc/>
        public IWebElement FindElement(By by)
        {
            return Element.FindElement(by);
        }

        /// <inheritdoc/>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return Element.FindElements(by);
        }

        /// <inheritdoc/>
        public string GetAttribute(string attributeName)
        {
            return Element.GetAttribute(attributeName);
        }

        /// <inheritdoc/>
        public string GetCssValue(string propertyName)
        {
            return Element.GetCssValue(propertyName);
        }

        /// <inheritdoc/>
        public string GetProperty(string propertyName)
        {
            return Element.GetProperty(propertyName);
        }

        /// <inheritdoc/>
        public void SendKeys(string text)
        {
            Element.SendKeys(text);
        }

        /// <inheritdoc/>
        public void Submit()
        {
            Element.Submit();
        }

        /// <summary>
        /// Retrieves the value and tries to cast it to the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetValue<T>() where T:IConvertible
        {
            var value = Element.GetProperty("value");

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
        /// Sets the 'value' of the input element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public virtual void SetValue<T>(T value) where T:IConvertible
        {
            var asString = (string)Convert.ChangeType(value, typeof(string));
            Element.Clear();

            if (!String.IsNullOrEmpty(asString))
                Element.SendKeys(asString);
        }

        #endregion
    }
}