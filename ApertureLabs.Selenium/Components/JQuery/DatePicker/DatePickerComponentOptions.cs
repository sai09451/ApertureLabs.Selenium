using System;

namespace ApertureLabs.Selenium.Components.JQuery.DatePicker
{
    /// <summary>
    /// DatePickerComponentOptions.
    /// </summary>
    public class DatePickerComponentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatePickerComponentOptions"/> class.
        /// </summary>
        public DatePickerComponentOptions()
        {
            AnimationDuration = TimeSpan.FromMilliseconds(500);
            DateFormat = "MM/dd/yyyy";
        }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets the date format used by the DatePicker.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        public string DateFormat { get; set; }
    }
}
