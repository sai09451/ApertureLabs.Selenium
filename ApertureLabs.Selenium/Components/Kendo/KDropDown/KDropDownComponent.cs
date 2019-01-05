using ApertureLabs.Selenium.Components.Shared.Animatable;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Kendo dropdown widget.
    /// </summary>
    public class KDropDownComponent : BaseKendoComponent,
        IAnimatableComponent<KDropDownAnimationOptions>
    {
        #region Fields

        private readonly KDropDownAnimationOptions animationData;

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
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        /// <param name="animationData"></param>
        public KDropDownComponent(IWebDriver driver,
            By selector,
            DataSourceOptions dataSourceOptions,
            KDropDownAnimationOptions animationData)
            : base(driver, selector, dataSourceOptions)
        {
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
            return SelectedOption.Text.Trim();
        }

        /// <summary>
        /// Sets the selected item.
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetSelectedItem(string value)
        {
            Expand();

            var newValEl = OptionsElements
                .FirstOrDefault(e => e.TextHelper().InnerText == value);

            if (newValEl == null)
                throw new NoSuchElementException();

            newValEl.Click();

            WaitForAnimationEnd();
        }

        /// <summary>
        /// Returns the wrapped select element that KDropDownList
        /// uses internally.
        /// </summary>
        /// <returns></returns>
        public virtual SelectElement GetSelectElement()
        {
            var wrappedElementTagName = WrappedElement.TagName;
            var selectElement = new SelectElement(WrappedElement);

            return selectElement;
        }

        /// <inheritdoc/>
        public void WaitForAnimationStart(
            KDropDownAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            GetPromiseForKendoEvent(IsExpanded() ? "open" : "close")
                .Wait(TimeSpan.FromSeconds(30));
        }

        /// <inheritdoc/>
        public void WaitForAnimationEnd(
            KDropDownAnimationOptions animationData = null)
        {
            var data = animationData ?? this.animationData;

            if (!data.AnimationsEnabled)
                return;

            // Wait the animation duration plus two seconds.
            var timeSpanWithTolerance = data.AnimationDuration
                + TimeSpan.FromSeconds(2);

            GetPromiseForKendoEvent(IsExpanded() ? "close" : "open")
                .Wait(timeSpanWithTolerance);
        }

        /// <inheritdoc/>
        public bool IsCurrentlyAnimating(
            KDropDownAnimationOptions animationData = null)
        {
            var opts = animationData ?? this.animationData;

            if (!opts.AnimationsEnabled)
                return false;

            try
            {
                WaitForAnimationEnd(opts);
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Expands the dropdown if not already expanded.
        /// </summary>
        protected void Expand()
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
        protected void Close()
        {
            if (IsExpanded())
            {
                var containerElement = ContainerElement;
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
