using System;
using ApertureLabs.Selenium.Extensions;
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
        /// Configuration on how to handle the datasource.
        /// </summary>
        protected readonly DataSourceOptions dataSourceOptions;

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        public BaseKendoComponent(IWebDriver driver,
            By selector,
            DataSourceOptions dataSourceOptions)
            : base(driver, selector)
        {
            this.dataSourceOptions = dataSourceOptions
                ?? throw new ArgumentNullException(nameof(dataSourceOptions));
        }

        #endregion

        #region Properties

        #region Elements

        /// <summary>
        /// The 'busy' or loading element displayed on the page.
        /// </summary>
        protected virtual IWebElement PageLoadingIndicator => WrappedDriver.FindElement(dataSourceOptions.PageLoadingSelector);

        /// <summary>
        /// The 'busy' or loading element displayed on the container.
        /// </summary>
        protected virtual IWebElement ContainerLoadingIndicator => WrappedDriver.FindElement(dataSourceOptions.ContainerLoadingSelector);

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
            if (!dataSourceOptions.RemoteDataSource)
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
        /// <param name="wait"></param>
        protected virtual void WaitForKendoEvent(string eventName,
            TimeSpan? wait = null)
        {
            var script =
                $"var callback = arguments[arguments.length - 1];" +
                $"var $el = $(arguments[0]);" +
                $"var dropdown = $el.data().kendoDropDownList;" +
                $"var unbindCallback = function () {{" +
                    $"dropdown.unbind('{eventName}', unbindCallback)" + 
                    $"callback();" +
                $"}};" +
                $"dropdown.bind('{eventName}', unbindCallback)";

            WrappedDriver.Wait(wait ?? TimeSpan.FromSeconds(30))
                .Until(d =>
                {
                    WrappedDriver.ExecuteAsyncScript(script,
                        wait,
                        WrappedElement);

                    return true;
                });
        }

        #endregion
    }
}
