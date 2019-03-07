using OpenQA.Selenium;
using System;
using System.Globalization;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// Base class for CheckboxElement and RadioElement. Internal use only.
    /// </summary>
    public class RadioCheckboxBase : InputElement
    {
        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="element"></param>
        internal RadioCheckboxBase(IWebElement element) : base(element)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Returns true if the element is checked and false if not.
        /// </summary>
        public bool IsChecked => Boolean.Parse(GetProperty("checked") ?? "false");

        #endregion

        #region Methods

        /// <summary>
        /// Will assert that the element is checked or unchecked depending on
        /// the argument. NOTE: This will 'click' the element to toggle the
        /// state so if it's not clickable this will throw an exception.
        /// </summary>
        /// <param name="checked"></param>
        /// <exception cref="StaleElementReferenceException"></exception>
        /// <exception cref="InvalidElementStateException"></exception>
        /// <exception cref="ElementNotInteractableException"></exception>
        /// <exception cref="ElementNotVisibleException"></exception>
        public void Check(bool @checked)
        {
            if (IsChecked != @checked)
            {
                Click();
            }
        }

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member
        /// <summary>
        /// Sets the value of the property checked.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        [Obsolete("Use the method 'Checked' instead.")]
        public override void SetValue<T>(T value)
        {
            var asBool = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
            Check(asBool);
        }

        /// <summary>
        /// Returns the value or the property 'checked'.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [Obsolete("Use the property 'IsChecked' instead.")]
        public override T GetValue<T>()
        {
            return base.GetValue<T>();
        }
#pragma warning restore CS0809 // Obsolete member overrides non-obsolete member

        #endregion
    }
}
