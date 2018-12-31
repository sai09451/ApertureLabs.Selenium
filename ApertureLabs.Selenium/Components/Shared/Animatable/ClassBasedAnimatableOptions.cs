using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Options for class based animations.
    /// </summary>
    public class ClassBasedAnimatableOptions
    {
        /// <summary>
        /// A list of classes that are added when animating the component.
        /// </summary>
        public IEnumerable<string> AnimatingClasses { get; set; }

        /// <summary>
        /// The default time the animation should take to complete.
        /// </summary>
        public TimeSpan AnimationDuration { get; set; }
    }
}
