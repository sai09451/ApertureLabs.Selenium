using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Shared.Animatable
{
    /// <summary>
    /// Default implementation of 
    /// </summary>
    public class ClassBasedAnimatableComponent : PageComponent
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public ClassBasedAnimatableComponent(IWebDriver driver, By selector)
            : base(driver, selector)
        { }
    }
}
