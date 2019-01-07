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

        #region Selectors

        private readonly By OptionsSelector = By.CssSelector("ul > li");
        private readonly By SelectedOptionsSelector = By.CssSelector(".k-multiselect-wrap li span:not('.k-icon')");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        /// <param name="animationData"></param>
        public KMultiSelectComponent(IWebDriver driver,
            By selector,
            DataSourceOptions dataSourceOptions,
            KMultiSelectAnimationOptions animationData)
            : base(driver, selector, dataSourceOptions)
        {
            this.animationData = animationData;
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
        public virtual void SelectItem(string item)
        {
            if (GetSelectedOptions().Contains(item))
                return;

            var el = OptionElements.FirstOrDefault(
                e => String.Equals(
                    e.TextHelper().InnerText,
                    item,
                    StringComparison.Ordinal));

            // Throw if the element doesn't exist.
            if (el == null)
                throw new NoSuchElementException();

            Expand();
            el.Click();
            WrappedDriver.Wait(animationData.AnimationDuration)
                .Until(d => !el.Displayed);
        }

        /// <summary>
        /// Deselects an item if it's selected.
        /// </summary>
        /// <param name="item"></param>
        public virtual void DeselectItem(string item)
        {
            throw new NotImplementedException();
        }

        /// <inheritDoc/>
        public virtual void WaitForAnimationStart(KMultiSelectAnimationOptions animationData = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritDoc/>
        public virtual void WaitForAnimationEnd(KMultiSelectAnimationOptions animationData = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritDoc/>
        public virtual bool IsCurrentlyAnimating(KMultiSelectAnimationOptions animationData = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Expands the widget if not already expanded.
        /// </summary>
        protected virtual void Expand()
        {
            if (!IsExpanded)
            {
                ContainerElement.Click();

                WrappedDriver.Wait(animationData.AnimationDuration)
                    .Until(d => IsExpanded);
            }
        }

        #endregion
    }
}
