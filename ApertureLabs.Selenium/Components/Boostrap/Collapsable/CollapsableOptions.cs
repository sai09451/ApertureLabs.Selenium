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
        #region Constructors

        private CollapsableOptions()
        {
            AnimationDuration = TimeSpan.FromMilliseconds(750);
            AnimationsEnabled = true;
            AnimationSelectors = new[] { By.CssSelector(".collapsing") };
            CollapsableContainerSelector = null;
            CloseSelectors = Array.Empty<By>();
            IsOpenSelector = By.CssSelector(".collapse.show");
            OpenSelectors = Array.Empty<By>();
            ToggleSelectors = Array.Empty<By>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsableOptions"/> class.
        /// </summary>
        public CollapsableOptions(By collapsableContainerSelector,
            IEnumerable<By> toggleSelectors)
            : this()
        {
            CollapsableContainerSelector = collapsableContainerSelector
                ?? throw new ArgumentNullException(nameof(collapsableContainerSelector));
            ToggleSelectors = toggleSelectors
                ?? throw new ArgumentNullException(nameof(toggleSelectors));

            OpenSelectors = Array.Empty<By>();
            CloseSelectors = Array.Empty<By>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsableOptions"/> class.
        /// </summary>
        /// <param name="collapsableContainerSelector">The collapsable container selector.</param>
        /// <param name="openSelectors">The open selectors.</param>
        /// <param name="closeSelectors">The close selectors.</param>
        /// <exception cref="ArgumentNullException">
        /// collapsableContainerSelector
        /// or
        /// openSelectors
        /// or
        /// closeSelectors
        /// </exception>
        public CollapsableOptions(By collapsableContainerSelector,
            IEnumerable<By> openSelectors,
            IEnumerable<By> closeSelectors)
            : this()
        {
            CollapsableContainerSelector = collapsableContainerSelector
                ?? throw new ArgumentNullException(nameof(collapsableContainerSelector));
            OpenSelectors = openSelectors
                ?? throw new ArgumentNullException(nameof(openSelectors));
            CloseSelectors = closeSelectors
                ?? throw new ArgumentNullException(nameof(closeSelectors));
        }

        #endregion

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
        /// The selector used when the CollapsableContainer is open.
        /// </summary>
        public By IsOpenSelector { get; set; }

        /// <summary>
        /// The expected duration of the animation (Usually five seconds is
        /// added onto this time for wait conditions).
        /// </summary>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Classes used to represent when the component is being animated.
        /// </summary>
        public IEnumerable<By> AnimationSelectors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [animations enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [animations enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool AnimationsEnabled { get; set; }
    }
}
