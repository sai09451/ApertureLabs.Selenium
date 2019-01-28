﻿using System;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Options that configures which classes to observer when determining if
    /// the dropdown is animating.
    /// </summary>
    public class KDropDownAnimationOptions
    {
        /// <inheritdoc/>
        public TimeSpan AnimationDuration { get; set; }

        /// <inheritdoc/>
        public bool AnimationsEnabled { get; set; }

        /// <summary>
        /// Returns a default KDropDownAnimations object with AnimationsEnabled
        /// and AnimationDuration set to 200ms.
        /// </summary>
        public static KDropDownAnimationOptions Default()
        {
            return new KDropDownAnimationOptions
            {
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                AnimationsEnabled = true
            };
        }
    }
}
