using ApertureLabs.Selenium.PageObjects;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// An animatable component that uses events to determine when it
    /// enters/exits an animated state.
    /// </summary>
    public interface IEventBasedAnimatableComponent : IPageComponent
    {
        /// <summary>
        /// Determines if the element is in an animated state.
        /// </summary>
        /// <returns></returns>
        bool IsAnimating();
    }
}
