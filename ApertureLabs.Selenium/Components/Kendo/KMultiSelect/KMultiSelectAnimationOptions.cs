using System;

namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Animation options for the KMultiSelectComponent.
    /// </summary>
    public class KMultiSelectAnimationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KMultiSelectAnimationOptions"/> class.
        /// </summary>
        public KMultiSelectAnimationOptions()
        {
            AnimationDuration = TimeSpan.FromMilliseconds(500);
            AnimationsEnabled = true;
        }

        /// <summary>
        /// Gets the duration of the animation.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Gets a value indicating whether [animations enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animations enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AnimationsEnabled { get; set; }
    }
}
