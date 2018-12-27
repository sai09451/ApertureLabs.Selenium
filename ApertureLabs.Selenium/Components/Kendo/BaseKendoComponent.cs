using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Components.Kendo
{
    /// <summary>
    /// Base class
    /// </summary>
    public abstract class BaseKendoComponent : PageComponent
    {
        #region Fields

        #region Selectors

        /// <summary>
        /// Selector for the ajaxBusy element.
        /// </summary>
        protected readonly By BusySelector = By.CssSelector("ajaxBusy");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public BaseKendoComponent(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        /// <summary>
        /// The 'busy' or loading element displayed during an ajax call.
        /// </summary>
        protected virtual IWebElement BusyElement => WrappedDriver.FindElement(BusySelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the page is displaying the loading indicator or the
        /// component is loading/busy.
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsBusy()
        {
            return BusyElement.Displayed;
        }

        /// <summary>
        /// Waits for the busy indicator to appear (if not already visible)
        /// then dissapear.
        /// </summary>
        /// <param name="wait">Defaults to one minute</param>
        protected virtual void WaitForAjaxOperation(TimeSpan? wait = null)
        {
            wait = wait ?? TimeSpan.FromMinutes(1);
            var noWaitErrors = WrappedDriver.Wait(wait.Value)
                .TrySequentialWait(out var exception,
                    d => IsBusy(),
                    d => !IsBusy());

            if (!noWaitErrors)
                throw exception;
        }

        #endregion
    }
}
