using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Collapsable
{
    /// <summary>
    /// Contains classes and selectors for the Bootstrap CollapsableComponent.
    /// </summary>
    public class CollapsableOptions
    {
        /// <summary>
        /// Selectors for elements that ONLY expand the component.
        /// </summary>
        public IEnumerable<By> OpenSelectors { get; set; }

        /// <summary>
        /// Selectors for elements that ONLY collapse the component.
        /// </summary>
        public IEnumerable<By> CloseSelectors { get; set; }

        /// <summary>
        /// Selectors for elements that both expand and collapse the component.
        /// </summary>
        public IEnumerable<By> ToggleSelectors { get; set; }

        /// <summary>
        /// The selector for the element being expanded/collapsed.
        /// </summary>
        public By CollapsableContainerSelector { get; set; }

        /// <summary>
        /// The class used when the CollapsableContainer is open.
        /// </summary>
        public string OpenClass { get; set; }

        /// <summary>
        /// The expected duration of the animation (Usually five seconds is
        /// added onto this time for wait conditions).
        /// </summary>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Classes used to represent when the component is being animated.
        /// </summary>
        public IEnumerable<string> AnimationClasses { get; set; }

        /// <inheritdoc/>
        public bool AnimationsEnabled { get; set; }
    }
}
