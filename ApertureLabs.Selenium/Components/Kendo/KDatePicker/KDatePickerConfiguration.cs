using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ApertureLabs.Selenium.Components.Kendo.KDatePicker
{
    /// <summary>
    /// Configuration for the <see cref="KDatePickerComponent{T}"/>.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoConfiguration" />
    public class KDatePickerConfiguration : BaseKendoConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KDatePickerConfiguration"/> class.
        /// </summary>
        public KDatePickerConfiguration()
        {
            // Base properties.
            DataSource = new DataSourceOptions();
            ControlWithKeyboardInsteadOfMouse = false;

            // Current properties.
            EnterValueAsText = false;
            AnimationsEnabled = true;
            AnimationDuration = TimeSpan.FromSeconds(1);
            Depth = Depth.Month;
            Min = null;
            Max = null;
            DateTimeFormats = new[] { "M/d/yyyy" };
            DisabledDates = Array.Empty<DateTime>();
            EnabledDates = Array.Empty<DateTime>();
            Culture = CultureInfo.CreateSpecificCulture("en-US");
        }

        /// <summary>
        /// Gets or sets a value indicating whether interacting with the
        /// element via the input element. If false then value will be
        /// attempted to be set via the calendar modal.
        /// </summary>
        /// <value>
        ///   <c>true</c> if interacting with the element via the input
        ///   element; otherwise, <c>false</c>.
        /// </value>
        bool EnterValueAsText { get; set; }

        /// <summary>
        /// Gets or sets the navigational depth. AKA which date range is
        /// displayed.
        /// </summary>
        /// <value>
        /// The depth.
        /// </value>
        public Depth Depth { get; set; }

        /// <summary>
        /// Gets or sets the starting 'depth' view.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public Depth Start { get; set; }

        /// <summary>
        /// How far back dates are displayed.
        /// </summary>
        /// <value>
        /// The minimum day a user can select.
        /// </value>
        public DateTime? Min { get; set; }

        /// <summary>
        /// How far forwards dates are displayed.
        /// </summary>
        /// <value>
        /// The maximum day a user can select.
        /// </value>
        public DateTime? Max { get; set; }

        /// <summary>
        /// Gets or sets the date time format.
        /// </summary>
        /// <value>
        /// The date time format.
        /// </value>
        public IEnumerable<string> DateTimeFormats { get; set; }

        /// <summary>
        /// Gets or sets the disabled dates.
        /// </summary>
        /// <value>
        /// The disabled dates.
        /// </value>
        public IEnumerable<DateTime> DisabledDates { get; set; }

        /// <summary>
        /// Gets or sets the enabled dates.
        /// </summary>
        /// <value>
        /// The enabled dates.
        /// </value>
        public IEnumerable<DateTime> EnabledDates { get; set; }

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [animations enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animations enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AnimationsEnabled { get; set; }
    }
}
