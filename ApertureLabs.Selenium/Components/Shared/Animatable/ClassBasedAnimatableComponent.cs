using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Default implementation of 
    /// </summary>
    public class ClassBasedAnimatableComponent : PageComponent, IAnimatableComponent<ClassBasedAnimatableOptions>
    {
        private readonly ClassBasedAnimatableOptions DefaultOptions;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="defaultOptions"></param>
        public ClassBasedAnimatableComponent(IWebDriver driver,
            By selector,
            ClassBasedAnimatableOptions defaultOptions)
            : base(driver, selector)
        {
            DefaultOptions = defaultOptions;
        }

        public bool IsCurrentlyAnimating(ClassBasedAnimatableOptions animationData = null)
        {
            throw new System.NotImplementedException();
        }

        public void WaitForAnimationEnd(ClassBasedAnimatableOptions animationData = null)
        {
            throw new System.NotImplementedException();
        }

        public void WaitForAnimationStart(ClassBasedAnimatableOptions animationData = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
