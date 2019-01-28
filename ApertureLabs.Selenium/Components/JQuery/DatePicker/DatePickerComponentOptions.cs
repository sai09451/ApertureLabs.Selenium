using System;

namespace ApertureLabs.Selenium.Components.JQuery.DatePicker
{
    /// <summary>
    /// DatePickerComponentOptions.
    /// </summary>
    public class DatePickerComponentOptions
    {
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

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public static DatePickerComponentOptions Default()
        {
            return new DatePickerComponentOptions()
            {
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                DateFormat = "MM/dd/yyyy"
            };
        }
    }
}
