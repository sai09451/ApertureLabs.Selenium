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

        public PageObject(EventFiringWebDriver driver)
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
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref=""></exception>
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

            return this;
        }

        public bool IsStateValid()
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
