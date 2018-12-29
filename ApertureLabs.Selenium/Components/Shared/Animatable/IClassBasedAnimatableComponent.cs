namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Represents a component
    /// </summary>
    public interface IClassBasedAnimatableComponent<T> where T : class, IClassBasedAnimatableOptions
    {
        /// <summary>
        /// True if the element contains any of the 'animated' designated
        /// classes.
        /// </summary>
        /// <param name="options">Optional 'options' object.</param>
        /// <returns></returns>
        bool IsAnimating(T options = null);
    }
}
