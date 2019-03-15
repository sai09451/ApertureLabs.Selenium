using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    /// <summary>
    /// RadioElementSet
    /// </summary>
    public class RadioElementSet
    {
        #region Constructor

        /// <summary>
        /// Checks that each element is a radio button and shares the same name.
        /// </summary>
        /// <param name="radioButtons"></param>
        public RadioElementSet(IEnumerable<IWebElement> radioButtons)
        {
            if (!radioButtons.Any())
            {
                throw new ArgumentException($"{nameof(radioButtons)} " +
                    $"contained no elements.");
            }

            // Cast to input elements.
            var asRadioElements = radioButtons.Select(r => new RadioElement(r));
            var firstElName = asRadioElements.First().GetAttribute("name");

            // Validate the names of each element.
            var validNames = asRadioElements.All(r => r.GetGroup() == firstElName);

            if (!validNames)
            {
                throw new InvalidElementStateException();
            }

            // Elements are valid.
            Options = new List<RadioElement>(asRadioElements);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Options.
        /// </summary>
        public IReadOnlyList<RadioElement> Options { get; private set; }

        /// <summary>
        /// Gets the selected item or returns null if nothing is selected.
        /// </summary>
        public RadioElement SelectedOption => Options.FirstOrDefault(opt => opt.IsChecked);

        #endregion

        #region Methods

        /// <summary>
        /// Selects an option by its value.
        /// </summary>
        /// <param name="value"></param>
        public void SelectByValue(string value)
        {
            Options.First(opt => opt.GetValue<string>() == value).Check(true);
        }

        /// <summary>
        /// Selects an option by index, the order in which they appear in the
        /// html.
        /// </summary>
        /// <param name="index"></param>
        public void SelectByIndex(int index)
        {
            Options[index].Check(true);
        }

        #endregion
    }
}
