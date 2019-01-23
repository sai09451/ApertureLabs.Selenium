using System;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

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
        /// The options that define how to interact with the component.
        /// </summary>
        protected readonly BaseKendoConfiguration configuration;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="selector"></param>
        /// <param name="driver"></param>
        public BaseKendoComponent(BaseKendoConfiguration configuration,
            By selector,
            IWebDriver driver)
            : base(driver, selector)
        {
            this.configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        #endregion

        #region Properties

        #region Elements

        /// <summary>
        /// The 'busy' or loading element displayed on the page.
        /// </summary>
        protected virtual IWebElement PageLoadingIndicator => WrappedDriver.FindElement(configuration.DataSource.PageLoadingSelector);

        /// <summary>
        /// The 'busy' or loading element displayed on the container.
        /// </summary>
        protected virtual IWebElement ContainerLoadingIndicator => WrappedDriver.FindElement(configuration.DataSource.ContainerLoadingSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the page is displaying the loading indicator (page
        /// or container).
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsBusy()
        {
            return PageLoadingIndicator?.Displayed
                ?? ContainerLoadingIndicator?.Displayed
                ?? false;
        }

        /// <summary>
        /// If DataSourceOptions.RemoteDataSource is true then waits for the
        /// busy indicator to appear (if not already visible) then dissapear.
        /// </summary>
        /// <param name="wait">Defaults to one minute</param>
        protected virtual void WaitForLoadingOperation(TimeSpan? wait = null)
        {
            if (!configuration.DataSource.RemoteDataSource)
                return;

            wait = wait ?? TimeSpan.FromMinutes(1);
            var noWaitErrors = WrappedDriver.Wait(wait.Value)
                .TrySequentialWait(out var exception,
                    d => IsBusy(),
                    d => !IsBusy());

            if (!noWaitErrors)
                throw exception;
        }

        /// <summary>
        /// Since kendo events aren't compatible with native js events use this
        /// method to listen for kendo events.
        /// </summary>
        /// <param name="eventName">Name of the event to listen for.</param>
        protected virtual PromiseBody GetPromiseForKendoEvent(
            string eventName)
        {
            var script =
                "var callback = {resolve};" +
                "var $el = $({args}[0]);" +
                "var dropdown = $el.data().kendoDropDownList;" +
                "var unbindCallback = function () {" +
                    $"dropdown.unbind('{eventName}', unbindCallback);" +
                    "callback();" +
                "};" +
                $"dropdown.bind('{eventName}', unbindCallback);";

            var promise = new PromiseBody(WrappedDriver)
            {
                Arguments = new object[] { WrappedElement },
                Script = script
            };

            promise.Execute(WrappedDriver.JavaScriptExecutor());

            return promise;
        }

        #endregion
    }
}
