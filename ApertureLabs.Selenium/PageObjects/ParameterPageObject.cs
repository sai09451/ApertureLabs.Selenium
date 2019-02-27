using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.PageObjects
{
    public class ParameterPageObject : IParameterPageObject, IDisposable
    {
        #region Fields

        private bool assignedEventListeners;
        private bool disposedValue = false; // To detect redundant calls

        #endregion

        #region Constructor

        public ParameterPageObject(string uriMatcher, IWebDriver driver)
        {
            UriMatcher = uriMatcher
                ?? throw new ArgumentNullException(nameof(uriMatcher));
            WrappedDriver = driver
                ?? throw new ArgumentNullException(nameof(driver));
        }

        #endregion

        #region Properties

        public string UriMatcher { get; private set; }

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
            var matches = regex.Match(UriMatcher);

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
            throw new NotImplementedException();
        }

        private void OnNavigation(object sender, WebDriverNavigationEventArgs eventArgs)
        {
            if (IsStale())
            {
                Dispose();
            }
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
