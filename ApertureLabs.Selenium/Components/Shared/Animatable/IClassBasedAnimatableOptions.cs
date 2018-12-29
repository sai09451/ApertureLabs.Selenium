using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Contains classes that indicate if the element is being animated.
    /// </summary>
    public interface IClassBasedAnimatableOptions
    {
        /// <summary>
        /// A list of classes that may represent a 'stage' of the animation.
        /// </summary>
        IEnumerable<string> AnimationClasses { get; set; }
    }
}
