using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// Represents an INPUT element with a type of text that has the list.
    /// TODO: Possibly rename this class to 'DataListElement'.
    /// attribute set.
    /// </summary>
    public class InputDataListElement : InputElement
    {
        #region Fields

        private readonly By datalistSelector;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="InvalidElementStateException">
        /// Throws if the element doens't have a list attribute on it.
        /// </exception>
        public InputDataListElement(IWebElement element) : base(element)
        {
            var listHref = element.GetAttribute("list");

            if (String.IsNullOrEmpty(listHref))
                throw new InvalidElementStateException("Element missing the 'list' attribute.");

            datalistSelector = By.CssSelector("#" + listHref);

            if (DataListElement == null)
                throw new NoSuchElementException();
        }

        #endregion

        #region Properties

        private IWebElement DataListElement => this.GetDriver().FindElement(datalistSelector);

        /// <summary>
        /// Retrieves all possible options in the datalist element.
        /// </summary>
        public IList<IWebElement> Options => DataListElement.Children();

        #endregion

        #region Methods

        /// <summary>
        /// Sets the value by the index of the option. If the option has no
        /// value then its text is used.
        /// </summary>
        /// <param name="index"></param>
        public void SelectOptionByIndex(int index)
        {
            var option = Options[index];
            var newVal = GetValueOfOption(option);

            SetValue(newVal);
        }

        /// <summary>
        /// Finds the option matching the text and sets the input value to it.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="partialMatch"></param>
        public void SelectOptionByText(string text, bool partialMatch = false)
        {
            IWebElement option = null;

            if (partialMatch)
                option = Options.First(opt => opt.Text.Contains(text));
            else
                option = Options.First(opt => opt.Text == text);

            var newVal = GetValueOfOption(option);
            SetValue(newVal);
        }

        /// <summary>
        /// Sets the value based on the value of the option element.
        /// </summary>
        /// <param name="optionElement">Must be an option element.</param>
        public void SelectOptionByElement(IWebElement optionElement)
        {
            if (optionElement == null)
                throw new ArgumentNullException(nameof(optionElement));

            var newVal = GetValueOfOption(optionElement);
            SetValue(newVal);
        }

        private string GetValueOfOption(IWebElement element)
        {
            var val = element.GetAttribute("value");

            if (String.IsNullOrEmpty(val))
                val = element.Text;

            return val;
        }

        #endregion
    }
}
