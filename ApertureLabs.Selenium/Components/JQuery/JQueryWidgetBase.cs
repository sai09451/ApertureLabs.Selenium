﻿using System;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.JQuery
{
    /// <summary>
    /// Base class for JQuery Widgets.
    /// </summary>
    public abstract class JQueryWidgetBase : PageComponent
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="JQueryWidgetBase"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="selector">The selector.</param>
        public JQueryWidgetBase(
            IWebDriver driver,
            By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Waits for the <c>dataName</c> to be defined on the
        /// $(WrappedElement).data() object. Assumes that <c>WrappedElement</c>
        /// has been assigned to.
        /// </summary>
        /// <param name="dataName">
        /// Name of the property on the data object.
        /// </param>
        /// <param name="timeout">The timeout.</param>
        protected virtual void WaitForInitialization(string dataName,
            TimeSpan timeout)
        {
            var js = WrappedDriver.JavaScriptExecutor();

            var script =
                $"var el = arguments[0];" +
                $"return '{dataName}' in $(el).data();";

            WrappedDriver.Wait(timeout)
                .Until(d => (bool)js.ExecuteScript(script, WrappedElement));
        }

        #endregion
    }
}