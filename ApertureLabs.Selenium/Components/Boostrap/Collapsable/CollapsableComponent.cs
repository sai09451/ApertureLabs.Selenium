using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Components.Shared.Animatable;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Boostrap.Collapsable
{
    /// <summary>
    /// Represents a bootstrap collapsable.
    /// </summary>
    public class CollapsableComponent : PageComponent,
        IAnimatableComponent<CollapsableOptions>
    {
        #region Fields

        private readonly CollapsableOptions animationData;

        /// <summary>
        /// This event fires immediately when the show instance method is
        /// called.
        /// </summary>
        private static readonly string EventShowCollapse = "show.bs.collapse";

        /// <summary>
        /// This event is fired when a collapse element has been made visible
        /// to the user (will wait for CSS transitions to complete).
        /// </summary>
        private static readonly string EventShownCollapse = "shown.bs.collapse";

        /// <summary>
        /// This event is fired immediately when the hide method has been
        /// called.
        /// </summary>
        private static readonly string EventHideCollapse = "hide.bs.collapse";

        /// <summary>
        /// This event is fired when a collapse element has been hidden from
        /// the user (will wait for CSS transitions to complete).
        /// </summary>
        private static readonly string EventHiddenCollapse = "hidden.bs.collapse";

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="animationData"></param>
        public CollapsableComponent(IWebDriver driver,
            CollapsableOptions animationData)
            : base(driver, animationData.CollapsableContainerSelector)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));
            else if (animationData == null)
                throw new ArgumentNullException(nameof(animationData));
            else if (!animationData.AnimationClasses.Any())
                throw new ArgumentNullException(nameof(animationData.AnimationClasses));
            else if (String.IsNullOrEmpty(animationData.ClosedClass))
                throw new ArgumentNullException(nameof(animationData.ClosedClass));
            else if (String.IsNullOrEmpty(animationData.OpenClass))
                throw new ArgumentNullException(nameof(animationData.OpenClass));
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyList<IWebElement> OpenElements => animationData.OpenSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyList<IWebElement> CloseElements => animationData.CloseSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyList<IWebElement> ToggleElements => animationData.ToggleSelectors
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
        public virtual CollapsableComponent Open(IWebElement element = null)
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

                WrappedDriver
                    .Wait(animationData.AnimationDuration + TimeSpan.FromSeconds(2))
                    .Until(d => !IsCurrentlyAnimating());
            }

            return this;
        }

        /// <summary>
        /// Will click an element that will close the collapsable if not
        /// already closed and wait for the animation to finish.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public virtual CollapsableComponent Close(IWebElement element = null)
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
            var classes = WrappedElement.Classes();

            return classes.Contains(animationData.OpenClass)
                && !IsCollapsed()
                && !IsCurrentlyAnimating();
        }

        /// <summary>
        /// Checks if the element is closed.
        /// </summary>
        public virtual bool IsCollapsed()
        {
            return WrappedElement.Classes().Contains(animationData.ClosedClass)
                && !IsExpanded()
                && !IsCurrentlyAnimating();
        }

        /// <inheritdoc/>
        public virtual void WaitForAnimationStart(CollapsableOptions animationData = null)
        {
            WrappedElement.WaitForEvent(EventShowCollapse);
        }

        /// <inheritdoc/>
        public virtual void WaitForAnimationEnd(CollapsableOptions animationData = null)
        {
            WrappedElement.WaitForEvent(EventHiddenCollapse);
        }

        /// <inheritdoc/>
        public virtual bool IsCurrentlyAnimating(CollapsableOptions animationData = null)
        {
            var opts = animationData ?? this.animationData;

            return WrappedElement.Classes()
                .Any(c => opts.AnimationClasses.Contains(c, StringComparer.Ordinal));
        }

        #endregion
    }
}
