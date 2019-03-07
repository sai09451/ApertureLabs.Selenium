using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Collapsable
{
    /// <summary>
    /// Represents a bootstrap collapsable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.FluidPageComponent{T}" />
    public class CollapsableComponent<T> : FluidPageComponent<T>
    {
        #region Fields

        private readonly CollapsableOptions animationData;

        /// <summary>
        /// This event fires immediately when the show instance method is
        /// called.
        /// </summary>
        private readonly string EventShowCollapse = "show.bs.collapse";

        /// <summary>
        /// This event is fired when a collapse element has been hidden from
        /// the user (will wait for CSS transitions to complete).
        /// </summary>
        private readonly string EventHiddenCollapse = "hidden.bs.collapse";

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="animationData"></param>
        /// <param name="driver"></param>
        /// <param name="parent"></param>
        public CollapsableComponent(CollapsableOptions animationData,
            IWebDriver driver,
            T parent)
            : base(animationData.CollapsableContainerSelector, driver, parent)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));
            else if (animationData == null)
                throw new ArgumentNullException(nameof(animationData));
            else if (!animationData.AnimationsEnabled && !animationData.AnimationSelectors.Any())
                throw new ArgumentException("If animations are enabled animation selectors cannot be null/empty.", nameof(animationData));
            else if (animationData.IsOpenSelector == null)
                throw new ArgumentException("The IsOpenSelector property is required.", nameof(animationData));

            this.animationData = animationData;
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyCollection<IWebElement> OpenElements => animationData.OpenSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyCollection<IWebElement> CloseElements => animationData.CloseSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyCollection<IWebElement> ToggleElements => animationData.ToggleSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves all elements capable of opening the collapsable.
        /// </summary>
        /// <param name="excludeToggleElements"></param>
        /// <returns></returns>
        public virtual IReadOnlyList<IWebElement> GetAllOpenElements(
            bool excludeToggleElements = false)
        {
            var els = new List<IWebElement>(OpenElements);

            if (!excludeToggleElements)
                els.AddRange(ToggleElements);

            return els.AsReadOnly();
        }

        /// <summary>
        /// Retrieves all elements capable of closing the collapsable.
        /// </summary>
        /// <param name="excludeToggleElements"></param>
        /// <returns></returns>
        public virtual IReadOnlyList<IWebElement> GetAllCloseElements(
            bool excludeToggleElements = false)
        {
            var els = new List<IWebElement>(CloseElements);

            if (!excludeToggleElements)
                els.AddRange(ToggleElements);

            return els.AsReadOnly();
        }

        /// <summary>
        /// Will click an element that will open the collapsable if not
        /// already open and wait for the animation to finish.
        /// </summary>
        /// <param name="element">
        /// Optional. If null will use the first element identified by
        /// Options.OpenSelector or if none the first element identified by the
        /// Options.ToggleSelector.
        /// </param>
        /// <returns></returns>
        public virtual CollapsableComponent<T> Open(IWebElement element = null)
        {
            if (!IsExpanded())
            {
                if (element == null)
                {
                    if (OpenElements.Any())
                    {
                        OpenElements.First().Click();
                    }
                    else if (ToggleElements.Any())
                    {
                        ToggleElements.First().Click();
                    }
                    else
                    {
                        throw new NoSuchElementException("Failed to locate " +
                            "any elements to expand the collapsable element.");
                    }
                }
                else
                {
                    element.Click();
                }

                if (animationData.AnimationsEnabled)
                {
                    WrappedDriver
                        .Wait(animationData.AnimationDuration + TimeSpan.FromSeconds(1))
                        .TrySequentialWait(
                            out var exc,
                            d => IsCurrentlyAnimating(),
                            d => !IsCurrentlyAnimating(),
                            d => IsExpanded());
                }
            }

            return this;
        }

        /// <summary>
        /// Will click an element that will close the collapsable if not
        /// already closed and wait for the animation to finish.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual CollapsableComponent<T> Close(IWebElement element = null)
        {
            if (!IsExpanded())
            {
                if (element == null)
                {
                    if (CloseElements.Any())
                    {
                        CloseElements.First().Click();
                    }
                    else if (ToggleElements.Any())
                    {
                        ToggleElements.First().Click();
                    }
                    else
                    {
                        throw new NoSuchElementException("Failed to locate " +
                            "any elements to collpase the collapsable element.");
                    }
                }
                else
                {
                    element.Click();
                }

                WrappedDriver
                    .Wait(animationData.AnimationDuration + TimeSpan.FromSeconds(2))
                    .Until(d => !IsCurrentlyAnimating());
            }

            return this;
        }

        /// <summary>
        /// Checks if the element is open.
        /// </summary>
        public virtual bool IsExpanded()
        {
            var isExpanded = WrappedElement.Is(animationData.IsOpenSelector)
                && !IsCurrentlyAnimating();

            return isExpanded;
        }

        /// <summary>
        /// Checks if the element is closed.
        /// </summary>
        public virtual bool IsCollapsed()
        {
            return !IsExpanded() && !IsCurrentlyAnimating();
        }

        /// <summary>
        /// Waits for animation start.
        /// </summary>
        /// <param name="animationData">The animation data.</param>
        protected virtual void WaitForAnimationStart(
            CollapsableOptions animationData = null)
        {
            WrappedElement.WaitForEvent(EventShowCollapse);
        }

        /// <summary>
        /// Waits for animation end.
        /// </summary>
        /// <param name="animationData">The animation data.</param>
        protected virtual void WaitForAnimationEnd(
            CollapsableOptions animationData = null)
        {
            WrappedElement.WaitForEvent(EventHiddenCollapse);
        }

        /// <summary>
        /// Determines whether the <c>WrappedElement</c> is animating.
        /// </summary>
        /// <param name="animationData">The animation data.</param>
        /// <returns>
        ///   <c>true</c> if [is currently animating] [the specified animation data]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsCurrentlyAnimating(
            CollapsableOptions animationData = null)
        {
            var opts = animationData ?? this.animationData;

            // Check that animations are enabled and that there are animation
            // selectors.
            if (!opts.AnimationsEnabled
                || (opts.AnimationSelectors?.Any() ?? false))
            {
                return false;
            }

            // Check if any of the animation selectors match the wrapped
            // element.
            return opts
                .AnimationSelectors
                .Any(s => WrappedElement.Is(s));
        }

        #endregion
    }
}
