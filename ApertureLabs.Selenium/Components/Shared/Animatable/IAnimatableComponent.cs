namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Represents a component that can have an animation (css or js
    /// animation/transition).
    /// </summary>
    public interface IAnimatableComponent<T>
        where T : class, IAnimatableOptions
    {
        /// <summary>
        /// Waits for an animation to start.
        /// </summary>
        /// <param name="animationData"></param>
        void WaitForAnimationStart(T animationData = null);

        /// <summary>
        /// Waits for an animation to end.
        /// </summary>
        /// <param name="animationData"></param>
        void WaitForAnimationEnd(T animationData = null);

        /// <summary>
        /// Checks if an animation is currently running.
        /// </summary>
        /// <param name="animationData"></param>
        /// <returns></returns>
        bool IsCurrentlyAnimating(T animationData = null);
    }
}
