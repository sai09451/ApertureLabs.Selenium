using OpenQA.Selenium;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// Wrapper around the InputElement providing extra functionality for
    /// checkboxes.
    /// </summary>
    public class CheckboxElement : RadioCheckboxBase
    {
        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="InvalidElementStateException">
        /// Thrown when the element doens't have the type of checkbox.
        /// </exception>
        public CheckboxElement(IWebElement element) : base(element)
        {
            if (GetAttribute("type") != "checkbox")
                throw new InvalidElementStateException("Element must have type checkbox.");
        }

        #endregion
    }
}
