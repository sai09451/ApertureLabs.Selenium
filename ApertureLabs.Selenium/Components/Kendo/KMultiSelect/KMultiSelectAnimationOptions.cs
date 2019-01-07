using System;
using ApertureLabs.Selenium.Components.Shared.Animatable;

namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Animation options for the KMultiSelectComponent.
    /// </summary>
    public class KMultiSelectAnimationOptions : IAnimatableOptions
    {
        /// <inheritDoc/>
        public TimeSpan AnimationDuration { get; private set; }

        /// <inheritDoc/>
        public bool AnimationsEnabled { get; private set; }

        /// <summary>
        /// Default options for the KMultiSelectAnimationOptions.
        /// </summary>
        /// <returns></returns>
        public static KMultiSelectAnimationOptions Default()
        {
            return new KMultiSelectAnimationOptions
            {
                AnimationDuration = TimeSpan.FromMilliseconds(500),
                AnimationsEnabled = true
            };
        }
    }
}
