using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Components.Kendo.KDatePicker
{
    /// <summary>
    /// Represents the kendo datetime picker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoComponent{T}" />
    public class KDatePickerComponent<T> : BaseKendoComponent<T>
    {
        #region Fields

        #region Selectors

        private readonly By toggleSelector = By.CssSelector(".k-select");

        #endregion

        private readonly KDatePickerConfiguration datePickerConfiguration;

        #endregion

        #region Constructor

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

        private IWebElement ToggleElement => WrappedElement
            .FindElement(toggleSelector);

        #endregion

        #endregion

        #region Methods

        public override ILoadableComponent Load()
        {
            base.Load();
            InputContainerElement = WrappedElement.GetParentElement();

            // Locate the generated calendar modal element.
            var script = new JavaScript
            {
                Script =
                    "var $el = $(arguments[0]);" +
                    "return $el.data().kendoDatePicker.dateView.calendar.element",
                Arguments = new[] { new JavaScriptValue(WrappedElement) }
            };

            ModalContainerElement = script
                .Execute(WrappedDriver.JavaScriptExecutor())
                .ToWebElement();

            return this;
        }

        public virtual void Close()
        {
            if (IsExpanded())
                ToggleElement.Click();

            throw new NotImplementedException();
        }

        public virtual void Open()
        {
            throw new NotImplementedException();
        }

        public virtual DateTime? GetValue()
        {
            throw new NotImplementedException();
        }

        public virtual void SetValue(DateTime? value)
        {
            throw new NotImplementedException();
        }

        private bool IsExpanded()
        {
            return ModalContainerElement.Displayed;
        }

        #endregion
    }
}
