using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;

namespace ApertureLabs.Selenium.PageObjects
{
    public class PageObject : IPageObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// To detect redundant calls.
        /// </summary>
        private bool disposedValue = false;

        private bool isValid = false;

        #endregion

        #region Constructor

        /// <summary>
        /// If passing in an EventFiringWebDriver event listeners will be added.
        /// </summary>
        /// <param name="driver"></param>
        public PageObject(IWebDriver driver)
        {
            WrappedDriver = driver;
        }

        #endregion

        #region Properties

        public IWebDriver WrappedDriver { get; private set; }

        public Uri Uri { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Call this when
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">
        /// Occurs when trying to use after this instance has been disposed.
        /// </exception>
        /// <exception cref="LoadableComponentException">
        /// Thrown when calling <code>Load()</code> and not on the correct url.
        /// </exception>
        public ILoadableComponent Load()
        {
            if (!disposedValue)
            {
                throw new ObjectDisposedException(nameof(PageObject));
            }

            if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver)
            {
                eventFiringWebDriver.Navigated += OnNavigation;
            }

            // TODO: Fill out rest of page.

            return this;
        }

        public virtual bool IsStateValid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if the state is still valid after navigation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnNavigation(object sender, WebDriverNavigationEventArgs eventArgs)
        {
            // Set isValid to false, if this method reaches the end set to true.
            isValid = false;

            if (!WrappedDriver.Url.StartsWith(Uri.ToString()))
            {
                return;
            }

            // Passed all assertions, isValid is still true.
            isValid = true;
        }

        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver)
                    {
                        eventFiringWebDriver.Navigated -= OnNavigation;
                    }
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PageObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
