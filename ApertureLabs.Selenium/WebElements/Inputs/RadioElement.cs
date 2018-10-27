using OpenQA.Selenium;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// RadioElement
    /// </summary>
    public class RadioElement : RadioCheckboxBase
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public RadioElement(IWebElement element) : base(element)
        {
            if (GetAttribute("type") != "checkbox")
                throw new InvalidElementStateException("Element must have type checkbox.");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the radio group name.
        /// </summary>
        /// <returns></returns>
        public string GetGroup()
        {
            return GetAttribute("name");
        }

        #endregion
    }
}
