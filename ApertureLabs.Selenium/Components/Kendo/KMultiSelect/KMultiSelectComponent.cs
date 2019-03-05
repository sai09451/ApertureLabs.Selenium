using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Represents a kendo multi-select component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KMultiSelectComponent<T> : BaseKendoComponent<T>
    {
        #region Fields

        private readonly KMultiSelectAnimationOptions animationData;
        private readonly new KMultiSelectConfiguration configuration;

        #region Selectors

        private readonly By optionsSelector = By.CssSelector("ul > li");
        private readonly By selectedOptionsSelector = By.CssSelector(".k-multiselect-wrap li span:not(.k-icon)");
        private readonly By deleteSelector = By.CssSelector(".k-icon.k-delete");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KMultiSelectComponent{T}"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="parent">The parent.</param>
        public KMultiSelectComponent(By selector,
            IWebDriver driver,
            KMultiSelectConfiguration configuration,
            T parent)
            : base(configuration,
                  selector,
                  driver,
                  parent)
        {
            this.configuration = configuration;
            this.animationData = configuration.AnimationOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the widget is expanded.
        /// </summary>
        /// <returns></returns>
        protected bool IsExpanded => ListContainerElement.Displayed;

        private IWebElement ContainerElement => WrappedElement.GetParentElement();
        private IReadOnlyList<IWebElement> OptionElements => ListContainerElement
            .FindElements(optionsSelector);
        private IReadOnlyList<IWebElement> SelectedOptionElements => ContainerElement
            .FindElements(selectedOptionsSelector);

        private IWebElement ListContainerElement
        {
            get
            {
                var script = new JavaScript
                {
                    Script = "var $el = $(arguments[0]);" +
                        "if ($el.length === 0) {" +
                            "throw new Error('Argument was null.');" +
                        "}" +
                        "return $el.data().kendoMultiSelect.list[0];",
                    Arguments = new[] { new JavaScriptValue(WrappedElement) }
                };

                return script
                    .Execute(WrappedDriver.JavaScriptExecutor())
                    .ToWebElement();
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

            Expand();

            var el = OptionElements.FirstOrDefault(
                e => String.Equals(
                    e.TextHelper().InnerText,
                    item,
                    stringComparison));

            // Throw if the element doesn't exist.
            if (el == null)
                throw new NoSuchElementException();

            // Use keyboard or mouse depending on config.
            if (base.configuration.ControlWithKeyboardInsteadOfMouse)
                el.SendKeys(Keys.Enter);
            else
                el.Click();

            if (configuration.AutoClose)
            {
                // Wait until the dropdown is hidden.
                WrappedDriver
                    .Wait(animationData.AnimationDuration)
                    .Until(d => !el.Displayed);
            }

            // Wait until the new item is present in the list.
            WrappedDriver
                .Wait(animationData.AnimationDuration)
                .Until(
                    d => GetSelectedOptions().Any(
                        opt => String.Equals(opt, item, stringComparison)));
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

            var elementToSelect = SelectedOptionElements.FirstOrDefault(
                e => String.Equals(
                    e.TextHelper().InnerText,
                    item,
                    stringComparison));

            // Verify the element exists.
            if (elementToSelect == null)
                throw new NoSuchElementException();

            var deleteElement = elementToSelect
                .GetParentElement()
                .FindElement(deleteSelector);

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
        protected virtual void WaitForAnimationStart(
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
        protected virtual void WaitForAnimationEnd(
            KMultiSelectAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            WrappedDriver
                .Wait(data.AnimationDuration)
                .Until(d => IsExpanded);
        }

        /// <inheritDoc/>
        protected virtual bool IsCurrentlyAnimating(
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
                if (base.configuration.ControlWithKeyboardInsteadOfMouse)
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
                if (base.configuration.ControlWithKeyboardInsteadOfMouse)
                    ContainerElement.SendKeys(Keys.Escape);
                else
                    ContainerElement.Click();

                WaitForAnimationEnd();
            }
        }

        #endregion
    }
}
