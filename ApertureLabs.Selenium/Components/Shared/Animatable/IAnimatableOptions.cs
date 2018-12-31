using System;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Base interface for IAnimatableComponent generic argument.
    /// </summary>
    public interface IAnimatableOptions
    {
        /// <summary>
        /// The duration of the animation.
        /// </summary>
        TimeSpan AnimationDuration { get; }

        /// <summary>
        /// Used to check if animations are enabled for the element.
        /// </summary>
        bool AnimationsEnabled { get; }
    }
}
