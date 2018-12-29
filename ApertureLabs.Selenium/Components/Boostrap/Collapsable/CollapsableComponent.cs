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
        IClassBasedAnimatableComponent<CollapsableOptions>
    {
        #region Fields

        private readonly CollapsableOptions options;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="options"></param>
        public CollapsableComponent(IWebDriver driver, CollapsableOptions options)
            : base(driver, options.CollapsableContainerSelector)
        {
            if (driver == null)
                throw new ArgumentNullException(nameof(driver));
            else if (options == null)
                throw new ArgumentNullException(nameof(options));
            else if (!options.AnimationClasses.Any())
                throw new ArgumentNullException(nameof(options.AnimationClasses));
            else if (String.IsNullOrEmpty(options.ClosedClass))
                throw new ArgumentNullException(nameof(options.ClosedClass));
            else if (String.IsNullOrEmpty(options.OpenClass))
                throw new ArgumentNullException(nameof(options.OpenClass));
        }

        #endregion

        #region Properties

        #region Elements

        private IReadOnlyList<IWebElement> OpenElements => options.CollapsableOpenSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyList<IWebElement> CloseElements => options.CollapsableCloseSelectors
            .SelectMany(s => WrappedDriver.FindElements(s))
            .ToList();

        private IReadOnlyList<IWebElement> ToggleElements => options.CollapsableToggleSelectors
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
        public IReadOnlyList<IWebElement> GetAllOpenElements(
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
        public IReadOnlyList<IWebElement> GetAllCloseElements(
            bool excludeToggleElements = false)
        {
            var els = new List<IWebElement>(CloseElements);

            if (!excludeToggleElements)
                els.AddRange(ToggleElements);

            return els.AsReadOnly();
        }

        /// <summary>
        /// Will click an element that will open the collapsable if not
        /// already open.
        /// </summary>
        /// <param name="element">
        /// Optional. If null will use the first element identified by
        /// Options.OpenSelector or if none the first element identified by the
        /// Options.ToggleSelector.
        /// </param>
        /// <returns></returns>
        public CollapsableComponent Open(IWebElement element = null)
        {
            if (!IsOpen())
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
                    .Wait(options.AnimationDuration + TimeSpan.FromSeconds(2))
                    .Until(d => !IsAnimating());
            }

            return this;
        }

        /// <summary>
        /// Will click an element that will close the collapsable if not
        /// already closed.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public CollapsableComponent Close(IWebElement element = null)
        {
            if (!IsOpen())
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
                    .Wait(options.AnimationDuration + TimeSpan.FromSeconds(2))
                    .Until(d => !IsAnimating());
            }

            return this;
        }

        /// <summary>
        /// Checks if the element is open.
        /// </summary>
        public bool IsOpen()
        {
            var classes = WrappedElement.Classes();

            return classes.Contains(options.OpenClass)
                && !IsClosed()
                && !IsAnimating();
        }

        /// <summary>
        /// Checks if the element is closed.
        /// </summary>
        public bool IsClosed()
        {
            return WrappedElement.Classes().Contains(options.ClosedClass)
                    && !IsOpen()
                    && !IsAnimating();
        }

        /// <summary>
        /// Determines if the component is being animated.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool IsAnimating(CollapsableOptions options = null)
        {
            var opts = options ?? this.options;

            return WrappedElement.Classes()
                .Any(c => opts.AnimationClasses.Contains(c));
        }

        #endregion
    }
}
