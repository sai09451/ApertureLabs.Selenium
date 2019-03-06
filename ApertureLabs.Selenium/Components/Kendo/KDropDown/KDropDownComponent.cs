using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Kendo dropdown widget.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KDropDownComponent<T> : BaseKendoComponent<T>
    {
        #region Fields

        private readonly KDropDownAnimationOptions animationData;
        private readonly BaseKendoConfiguration configuration;

        #region Selectors

        private readonly By WrappedDropdownSelector = By.CssSelector("select");
        private readonly By OptionsSelector = By.CssSelector("li");
        private readonly By SelectedOptionSelector = By.CssSelector(".k-state-selected");
        private readonly By OpenAnimationFinishedSelector = By.CssSelector(".k-state-active");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// The selector should target the original element that's been wrapped
        /// by kendo.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="animationData">The animation data.</param>
        /// <param name="parent">The parent.</param>
        public KDropDownComponent(BaseKendoConfiguration configuration,
            By selector,
            IWebDriver driver,
            KDropDownAnimationOptions animationData,
            T parent)
            : base(configuration,
                  selector,
                  driver,
                  parent)
        {
            this.configuration = configuration;
            this.animationData = animationData;
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement ContainerElement => WrappedElement.GetParentElement();

        private IReadOnlyList<IWebElement> OptionsElements => ListContainerElement.FindElements(OptionsSelector);

        private IWebElement SelectedOption => ListContainerElement.FindElement(SelectedOptionSelector);

        private IWebElement ListContainerElement
        {
            get
            {
                var script =
                    "var $el = $(arguments[0]);" +
                    "return $el.data().kendoDropDownList.list[0];";

                return WrappedDriver.ExecuteJavaScript<IWebElement>(
                    script,
                    WrappedElement);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override ILoadableComponent Load()
        {
            base.Load();

            // Verify the container element has the k-dropdown class on it.
            if (!ContainerElement.Classes().Contains("k-dropdown"))
            {
                throw new InvalidElementStateException("The container " +
                    "element is missing the k-dropdown class.");
            }

            return this;
        }

        /// <summary>
        /// Returns true if the component is expanded.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExpanded()
        {
            return ListContainerElement.Displayed;
        }

        /// <summary>
        /// Returns the list of items in the dropdown.
        /// </summary>
        /// <returns></returns>
        public virtual IList<string> GetItems()
        {
            return OptionsElements.Select(oe => oe.Text).ToList();
        }

        /// <summary>
        /// Returns the currently selected item.
        /// </summary>
        /// <returns></returns>
        public virtual string GetSelectedItem()
        {
            return SelectedOption.TextHelper().InnerText;
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stringComparison"></param>
        public virtual void SetSelectedItem(string value,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            // Check if already set.
            var isAlreadySet = String.Equals(
                GetSelectedItem(),
                value,
                stringComparison);

            if (isAlreadySet)
                return;

            Expand();

            var newValEl = OptionsElements
                .FirstOrDefault(e => String.Equals(
                    e.TextHelper().InnerText,
                    value,
                    stringComparison));

            if (newValEl == null)
                throw new NoSuchElementException();

            if (configuration.ControlWithKeyboardInsteadOfMouse)
            {
                // Check if the element is above or below.
                var currentValEl = OptionsElements.First(
                    e => e.Classes().Contains("k-state-selected"));

                var currentIndex = currentValEl.GetIndexRelativeToSiblings();
                var newIndex = newValEl.GetIndexRelativeToSiblings();

                // Determine which key to use and how many times to press it.
                var difference = currentIndex - newIndex;
                var magnitude = Math.Abs(difference);
                var key = difference > 0 ? Keys.Up : Keys.Down;

                // Keep sending the key needed.
                for (var i = 0; i < magnitude; i++)
                    ContainerElement.SendKeys(key);

                ContainerElement.SendKeys(Keys.Enter);
            }
            else
            {
                newValEl.Click();
            }

            WaitForAnimationEnd();
        }

        /// <summary>
        /// Waits for animation start.
        /// </summary>
        /// <param name="animationData">The animation data.</param>
        protected virtual void WaitForAnimationStart(
            KDropDownAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            GetPromiseForKendoEvent(IsExpanded() ? "open" : "close")
                .Wait(data.AnimationDuration);
        }

        /// <summary>
        /// Waits for animation end.
        /// </summary>
        /// <param name="animationData">The animation data.</param>
        protected virtual void WaitForAnimationEnd(
            KDropDownAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            GetPromiseForKendoEvent(IsExpanded() ? "close" : "open")
                .Wait(data.AnimationDuration);
        }

        /// <summary>
        /// Expands the dropdown if not already expanded.
        /// </summary>
        protected virtual void Expand()
        {
            if (!IsExpanded())
            {
                var containerElement = ContainerElement;
                containerElement.Click();

                WrappedDriver.Wait(animationData.AnimationDuration)
                    .Until(d => containerElement
                        .FindElements(OpenAnimationFinishedSelector)
                        .Any());
            }
        }

        /// <summary>
        /// Collapses the dropdown if not already collapsed.
        /// </summary>
        protected virtual void Close()
        {
            if (IsExpanded())
            {
                var containerElement = ContainerElement;

                if (configuration.ControlWithKeyboardInsteadOfMouse)
                    containerElement.SendKeys(Keys.Escape);
                else
                    containerElement.Click();

                WrappedDriver.Wait(animationData.AnimationDuration)
                    .Until(d => !containerElement
                        .FindElements(OpenAnimationFinishedSelector)
                        .Any());
            }
        }

        #endregion
    }
}
