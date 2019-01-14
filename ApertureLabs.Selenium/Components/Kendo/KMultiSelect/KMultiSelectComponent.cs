using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Components.Shared.Animatable;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Represents a kendo multi-select component.
    /// </summary>
    public class KMultiSelectComponent : BaseKendoComponent,
        IAnimatableComponent<KMultiSelectAnimationOptions>
    {
        #region Fields

        private readonly KMultiSelectAnimationOptions animationData;
        private readonly KMultiSelectConfiguration options;

        #region Selectors

        private readonly By OptionsSelector = By.CssSelector("ul > li");
        private readonly By SelectedOptionsSelector = By.CssSelector(".k-multiselect-wrap li span:not('.k-icon')");
        private readonly By DeleteSelector = By.CssSelector(".k-icon.k-delete");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="driver"></param>
        /// <param name="animationData"></param>
        /// <param name="configuration"></param>
        public KMultiSelectComponent(By selector,
            IWebDriver driver,
            KMultiSelectAnimationOptions animationData,
            KMultiSelectConfiguration configuration)
            : base(configuration,
                  selector,
                  driver)
        {
            this.animationData = animationData;
            this.options = configuration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the widget is expanded.
        /// </summary>
        /// <returns></returns>
        protected bool IsExpanded => ListContainerElement.Displayed;

        private IWebElement ContainerElement => WrappedElement.GetParentElement();
        private IReadOnlyList<IWebElement> OptionElements => ListContainerElement.FindElements(OptionsSelector);
        private IReadOnlyList<IWebElement> SelectedOptionElements => ContainerElement.FindElements(SelectedOptionsSelector);

        private IWebElement ListContainerElement
        {
            get
            {
                var script =
                    "var $el = $(arguments[0]);" +
                    "return $el.data().kendoMultiSelect.list[0];";

                return WrappedDriver.ExecuteJavaScript<IWebElement>(
                    script,
                    WrappedElement);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a list of all options.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetAllOptions()
        {
            return OptionElements.Select(e => e.TextHelper().InnerText);
        }

        /// <summary>
        /// Returns a list of all selected items.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetSelectedOptions()
        {
            return SelectedOptionElements.Select(e => e.TextHelper().InnerText);
        }

        /// <summary>
        /// Selects an item if not already selected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stringComparison"></param>
        public virtual void SelectItem(string item,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var isAlreadySelected = GetSelectedOptions()
                .Any(opt => String.Equals(
                    opt,
                    item,
                    stringComparison));

            if (isAlreadySelected)
                return;

            var el = OptionElements.FirstOrDefault(
                e => String.Equals(
                    e.TextHelper().InnerText,
                    item,
                    stringComparison));

            // Throw if the element doesn't exist.
            if (el == null)
                throw new NoSuchElementException();

            Expand();

            // Use keyboard or mouse depending on config.
            if (configuration.ControlWithKeyboardInsteadOfMouse)
                el.SendKeys(Keys.Enter);
            else
                el.Click();

            WrappedDriver.Wait(animationData.AnimationDuration)
                .Until(d => !el.Displayed);
        }

        /// <summary>
        /// Deselects an item if it's selected.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stringComparison"></param>
        public virtual void DeselectItem(string item,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            var isSelected = GetSelectedOptions()
                .Any(opt => String.Equals(
                    opt,
                    item,
                    stringComparison));

            if (!isSelected)
                return;

            var elementToSelect = SelectedOptionElements
                .FirstOrDefault(e => String.Equals(
                    e.TextHelper().InnerText,
                    item,
                    stringComparison));

            // Verify the element exists.
            if (elementToSelect == null)
                throw new NoSuchElementException();

            var deleteElement = elementToSelect.FindElement(DeleteSelector);
            deleteElement.Click();

            // Wait until the element is fully removed.
            WrappedDriver.Wait(TimeSpan.FromMilliseconds(500))
                .Until(
                    d => !GetSelectedOptions().Any(
                        opt => String.Equals(
                            opt,
                            item,
                            stringComparison)));
        }

        /// <inheritDoc/>
        public virtual void WaitForAnimationStart(
            KMultiSelectAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            var eventName = IsExpanded ? "close" : "open";
            var promise = GetPromiseForKendoEvent(eventName);

            WrappedDriver.Wait(
                    data.AnimationDuration,
                    new[] { typeof(TimeoutException) })
                .Until(d => promise.Finished);
        }

        /// <inheritDoc/>
        public virtual void WaitForAnimationEnd(
            KMultiSelectAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            WrappedDriver.Wait(data.AnimationDuration)
                .Until(d => IsExpanded);
        }

        /// <inheritDoc/>
        public virtual bool IsCurrentlyAnimating(
            KMultiSelectAnimationOptions animationData = null)
        {
            var animationContainer = ListContainerElement.GetParentElement();
            var initialHeight = animationContainer.GetCssValue("height");
            var result = false;

            WrappedDriver.Wait(
                    TimeSpan.FromMilliseconds(100),
                    new[] { typeof(TimeoutException) })
                .Until(d =>
                {
                    result = animationContainer.GetCssValue("height")
                        != initialHeight;

                    return result;
                });

            return result;
        }

        /// <summary>
        /// Expands the widget if not already expanded.
        /// </summary>
        protected virtual void Expand()
        {
            if (!IsExpanded)
            {
                // Use keyboard or mouse depending on config.
                if (configuration.ControlWithKeyboardInsteadOfMouse)
                    ContainerElement.SendKeys(Keys.ArrowDown);
                else
                    ContainerElement.Click();

                WaitForAnimationEnd();
            }
        }

        /// <summary>
        /// Collapses this widget if not already collapsed.
        /// </summary>
        protected virtual void Close()
        {
            if (IsExpanded)
            {
                // Use keyboard or mouse depending on config.
                if (configuration.ControlWithKeyboardInsteadOfMouse)
                    ContainerElement.SendKeys(Keys.Escape);
                else
                    ContainerElement.Click();

                WaitForAnimationEnd();
            }
        }

        #endregion
    }
}
