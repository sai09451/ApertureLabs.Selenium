using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.WebElements.Inputs;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ApertureLabs.Selenium.Components.Kendo.KDatePicker
{
    /// <summary>
    /// Represents the kendo datetime picker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoComponent{T}" />
    public class KDatePickerComponent<T> : BaseKendoComponent<T>,
        IExpandable,
        ICollapsable
    {
        #region Fields

        #region Selectors

        private readonly By toggleSelector = By.CssSelector(".k-select");
        private readonly By previousSelector = By.CssSelector(".k-nav-prev");
        private readonly By nextSelector = By.CssSelector(".k-nav-next");
        private readonly By depthSelector = By.CssSelector(".k-nav-fast");
        private readonly By enabledGridItemsSelector = By.CssSelector(".k-content tbody td:not(.k-other-month):not(.k-state-disabled)");
        private readonly By focusedGridItemSelector = By.CssSelector(".k-content tbody td.k-state-focused");

        #endregion

        private readonly KDatePickerConfiguration datePickerConfiguration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KDatePickerComponent{T}"/> class.
        /// </summary>
        /// <param name="datePickerConfiguration">The date picker configuration.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="parent">The parent.</param>
        public KDatePickerComponent(
            KDatePickerConfiguration datePickerConfiguration,
            By selector,
            IWebDriver driver,
            T parent)
            : base(datePickerConfiguration,
                  selector,
                  driver,
                  parent)
        {
            this.datePickerConfiguration = datePickerConfiguration;
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement ModalContainerElement { get; set; }

        private IWebElement InputContainerElement { get; set; }

        private IWebElement ToggleElement => InputContainerElement
            .FindElement(toggleSelector);

        private InputElement InputWrappedElement => new InputElement(
            WrappedElement);

        private IWebElement NextElement => ModalContainerElement
            .FindElement(nextSelector);

        private IWebElement PreviousElement => ModalContainerElement
            .FindElement(previousSelector);

        private IWebElement DepthElement => ModalContainerElement
            .FindElement(depthSelector);

        private IReadOnlyCollection<IWebElement> EnabledGridItems => ModalContainerElement
            .FindElements(enabledGridItemsSelector);

        private IWebElement FocusedGridItemElement => ModalContainerElement
            .FindElement(focusedGridItemSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// If overloaded don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        public override ILoadableComponent Load()
        {
            base.Load();
            InputContainerElement = WrappedElement.GetParentElement();

            // Locate the generated calendar modal element.
            var script = new JavaScript
            {
                Script =
                    "var $el = $(arguments[0]);" +
                    "return $el.data().kendoDatePicker.dateView.div[0]",
                Arguments = new[] { new JavaScriptValue(WrappedElement) }
            };

            ModalContainerElement = script
                .Execute(WrappedDriver.JavaScriptExecutor())
                .ToWebElement();

            return this;
        }

        /// <summary>
        /// Collapses the component. Does nothing if already collapsed.
        /// </summary>
        public virtual void Collapse()
        {
            if (IsExpanded())
            {
                ToggleElement.Click();
                WaitForAnimation();
            }
        }

        /// <summary>
        /// Expands the component if not already expanded.
        /// </summary>
        public virtual void Expand()
        {
            if (!IsExpanded())
            {
                ToggleElement.Click();
                WaitForAnimation();
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns></returns>
        public virtual DateTime? GetValue()
        {
            var valueStr = InputWrappedElement.GetValue<string>();

            if (String.IsNullOrEmpty(valueStr))
                return null;

            var parsed = DateTime.TryParseExact(valueStr,
                datePickerConfiguration.DateTimeFormats.ToArray(),
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime value);

            return parsed ? (DateTime?)value : null;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="value">The value.</param>
        public virtual void SetValue(DateTime? value)
        {
            if (value.HasValue)
            {
                var formatValueAs = datePickerConfiguration
                    .DateTimeFormats
                    .SelectRandom();

                if (datePickerConfiguration.ControlWithKeyboardInsteadOfMouse)
                {
                    InputWrappedElement.SetValue(
                        value.Value.ToString(
                            formatValueAs,
                            CultureInfo.CurrentCulture));
                }
                else
                {
                    var datetime = value.Value;

                    // Open the element.
                    Expand();

                    // Set year.
                    NavigateToDepth(Depth.Decade);
                    NavigateToItemWithText(datetime.Year);

                    // Set month.
                    NavigateToDepth(Depth.Year);
                    NavigateToItemWithText(
                        datetime.ToString(
                            "MMM",
                            CultureInfo.CurrentCulture));

                    // Set day.
                    NavigateToDepth(Depth.Month);
                    NavigateToItemWithText(datetime.Day);

                    // Collapse the element in case it doesn't autoclose.
                    Collapse();
                }
            }
            else
            {
                // Remove any value that was set.
                InputWrappedElement.Clear();
            }
        }

        private bool IsExpanded()
        {
            return ModalContainerElement.Displayed;
        }

        private void NavigateToItemWithText(string text)
        {
            var itemEl = EnabledGridItems
                .FirstOrDefault(e => text == e.TextHelper().InnerText);

            if (itemEl == null)
                throw new NoSuchElementException();

            itemEl.Click();
            WaitForAnimation();
        }

        private void NavigateToItemWithText(int item)
        {
            // Should already be expanded.
            // Check if in range.
            var inRange = false;

            while (!inRange)
            {
                var (min, max) = GetDisplayedItemRange();

                if (min > item)
                {
                    PreviousElement.Click();
                    WaitForAnimation();
                }
                else if (max < item)
                {
                    NextElement.Click();
                    WaitForAnimation();
                }
                else
                {
                    // Item is now in range.
                    inRange = true;
                }
            }

            NavigateToItemWithText(item.ToString(CultureInfo.CurrentCulture));
        }

        private void WaitForAnimation()
        {
            if (datePickerConfiguration.AnimationsEnabled)
                Thread.Sleep(datePickerConfiguration.AnimationDuration);
        }

        private (int min, int max) GetDisplayedItemRange()
        {
            var items = EnabledGridItems;

            var min = items.First()
                .TextHelper()
                .ExtractInteger();

            var max = items.Last()
                .TextHelper()
                .ExtractInteger();

            return (min, max);
        }

        private Depth GetDepth()
        {
            var depthText = DepthElement.TextHelper().InnerText;

            if (Regex.IsMatch(depthText, @"(?<century>^\d\d00-\d\d99$)"))
                return Depth.Century;
            else if (Regex.IsMatch(depthText, @"(?<decade>^\d{3}0-\d{3}9$)"))
                return Depth.Decade;
            else if (Regex.IsMatch(depthText, @"(?<year>^\d{4}$)"))
                return Depth.Year;
            else if (Regex.IsMatch(depthText, @"(?<month>^\w+ \d{4}$)"))
                return Depth.Month;
            else throw new NotImplementedException();
        }

        private void NavigateToDepth(Depth depth)
        {
            var currentDepth = (int)GetDepth();
            var desiredDepth = (int)depth;

            if (currentDepth == desiredDepth)
                return;

            var difference = desiredDepth - currentDepth;
            var navigatingUp = difference > 0;

            for (var steps = Math.Abs(difference); steps> 0; steps--)
            {
                if (navigatingUp)
                {
                    // Click the depth element.
                    DepthElement.Click();
                    WaitForAnimation();
                }
                else
                {
                    // Click the default focused element.
                    FocusedGridItemElement.Click();
                    WaitForAnimation();
                }
            }
        }

        #endregion
    }
}
