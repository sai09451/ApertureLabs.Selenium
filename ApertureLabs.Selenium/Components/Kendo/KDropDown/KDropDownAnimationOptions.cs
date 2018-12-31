using System;
using ApertureLabs.Selenium.Components.Shared.Animatable;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Options that configures which classes to observer when determining if
    /// the dropdown is animating.
    /// </summary>
    public class KDropDownAnimationOptions : IAnimatableOptions
    {
        /// <inheritdoc/>
        public TimeSpan AnimationDuration { get; set; }
    }
}
