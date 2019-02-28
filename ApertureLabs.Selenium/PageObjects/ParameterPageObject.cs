using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// A PageObject whose url has route parameters.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IParameterPageObject" />
    /// <seealso cref="System.IDisposable" />
    public class ParameterPageObject : IParameterPageObject, IDisposable
    {
        #region Fields

        private bool assignedEventListeners;
        private bool disposedValue = false; // To detect redundant calls

        #endregion

        #region Constructor

        public ParameterPageObject(string uriMatcher, IWebDriver driver)
        {
            Route = uriMatcher
                ?? throw new ArgumentNullException(nameof(uriMatcher));
            WrappedDriver = driver
                ?? throw new ArgumentNullException(nameof(driver));
        }

        #endregion

        #region Properties

        public string Route { get; private set; }

        public Uri Uri { get; private set; }

        public string WindowHandle { get; private set; }

        public IWebDriver WrappedDriver { get; private set; }

        #endregion

        #region Methods

        public bool Equals(IPageObject other)
        {
            throw new NotImplementedException();
        }

        public T GetParameterInfo<T>(string parameterName) where T : IConvertible
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RouteParameter> GetUrlParameters()
        {
            var regex = new Regex(@"{.*?}");
            var matches = regex.Match(Route);

            while (matches.Success)
            {
                var parameterInfo = new RouteParameter(matches.Value);
                yield return parameterInfo;
            }
        }

        public bool IsStale()
        {
            throw new NotImplementedException();
        }

        public ILoadableComponent Load()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(ParameterPageObject));

            if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver
                && !assignedEventListeners)
            {
                eventFiringWebDriver.Navigated += OnNavigation;
                assignedEventListeners = true;
            }

            if (Uri != null)
            {
                // Verify the Uri isn't null and the driver isn't already on
                // the url.
                var canUseUri = !String.Equals(
                    Uri?.ToString() ?? "",
                    WrappedDriver.Url,
                    StringComparison.OrdinalIgnoreCase);

                if (canUseUri)
                {
                    WrappedDriver.Navigate().GoToUrl(Uri.ToString());
                }
                else if (String.IsNullOrEmpty(WrappedDriver.Url)
                    || WrappedDriver.Url == "data:,"
                    || WrappedDriver.Url == "about:blank")
                {
                    WrappedDriver.Navigate().GoToUrl(Uri.ToString());
                }
            }

            throw new NotImplementedException();
        }

        private void OnNavigation(object sender, WebDriverNavigationEventArgs eventArgs)
        {
            if (IsStale())
            {
                Dispose();
            }
        }

        protected bool UriMatcherContainsWildCards()
        {
            return UriMatcher.Contains("*")
                || UriMatcher.Contains("{")
                || UriMatcher.Contains("}");
        }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (assignedEventListeners
                        && WrappedDriver is EventFiringWebDriver eventFiringWebDriver)
                    {
                        eventFiringWebDriver.Navigated -= OnNavigation;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        
        #endregion

        #endregion
    }
}
