using System;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Options that configures which classes to observer when determining if
    /// the dropdown is animating.
    /// </summary>
    public class KDropDownAnimationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KDropDownAnimationOptions"/> class.
        /// </summary>
        public KDropDownAnimationOptions()
        {
            AnimationDuration = TimeSpan.FromMilliseconds(500);
            AnimationsEnabled = true;
        }

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
