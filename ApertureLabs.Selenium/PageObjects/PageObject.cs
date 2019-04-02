using ApertureLabs.Selenium.Attributes;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Base class for PageObjects. Default implementation of IPageObject
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IPageObject" />
    /// <seealso cref="System.IDisposable" />
    public abstract class PageObject : IPageObject, IDisposable
    {
        #region Fields

        private IWebElement bodyElement;
        private bool disposedValue;
        private bool assignedEventListeners;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PageObject"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="baseUri">The base uri.</param>
        /// <param name="route">The route.</param>
        /// <exception cref="ArgumentNullException">
        /// driver
        /// or
        /// route
        /// or baseUri
        /// </exception>
        public PageObject(IWebDriver driver,
            Uri baseUri,
            UriTemplate route)
        {
            WrappedDriver = driver
                ?? throw new ArgumentNullException(nameof(driver));
            BaseUri = baseUri
                ?? throw new ArgumentNullException(nameof(baseUri));
            Route = route
                ?? throw new ArgumentNullException(nameof(route));

            disposedValue = false;
            assignedEventListeners = false;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="PageObject"/> class.
        /// </summary>
        ~PageObject()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the base URI. Used alongside the <see cref="P:ApertureLabs.Selenium.PageObjects.IPageObject.Route" /> to check
        /// if the current url is valid.
        /// </summary>
        /// <value>
        /// The base URI.
        /// </value>
        public virtual Uri BaseUri { get; protected set; }

        /// <summary>
        /// Used to match the Uri of the webpage. Is used to create a Regex
        /// object to verify the url of the page when calling <c>Load</c>.
        /// The syntax is identical to that of the RouteAttribute. Used with
        /// the <see cref="P:ApertureLabs.Selenium.PageObjects.IPageObject.BaseUri" /> property to check if the current url is
        /// valid.
        /// </summary>
        public virtual UriTemplate Route { get; protected set; }

        /// <summary>
        /// Gets the URI. Should be null until
        /// <see cref="M:OpenQA.Selenium.Support.UI.ILoadableComponent.Load" /> is
        /// called where it will be set to the Url the
        /// <see cref="T:OpenQA.Selenium.IWebDriver" />
        /// is on.
        /// </summary>
        public virtual Uri Uri { get; protected set; }

        /// <summary>
        /// Gets the window handle the page was originally loaded on.
        /// </summary>
        public virtual string WindowHandle { get; protected set; }

        /// <summary>
        /// Gets the <see cref="T:OpenQA.Selenium.IWebDriver" /> used to find
        /// this element.
        /// </summary>
        public virtual IWebDriver WrappedDriver { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Equalses the specified page object.
        /// </summary>
        /// <param name="pageObject">The page object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">pageObject</exception>
        public virtual bool Equals(IPageObject pageObject)
        {
            return GetHashCode() == (pageObject?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is PageObject pageObject
                ? Equals(pageObject)
                : base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing
        /// algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashCode = -1839585392;
            hashCode = hashCode
                * -1521134295
                + EqualityComparer<IWebDriver>.Default.GetHashCode(WrappedDriver);
            hashCode = hashCode
                * -1521134295
                + EqualityComparer<Uri>.Default.GetHashCode(Uri);
            hashCode = hashCode
                * -1521134295
                + EqualityComparer<UriTemplate>.Default.GetHashCode(Route);
            hashCode = hashCode
                * -1521134295
                + EqualityComparer<string>.Default.GetHashCode(WindowHandle);
            return hashCode;
        }

        /// <summary>
        /// Use to determine if a PageObject is still 'valid'.
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// If there are two page object instances; one for a login page and
        /// one for the home page, and the driver is on the home page the login
        /// page instance should return true while the home page instance
        /// should return true.
        /// </example>
        public virtual bool IsStale()
        {
            return bodyElement.IsStale();
        }

        /// <summary>
        /// Loads the component. Checks to see if the current url matches
        /// the Route and if not an exception is thrown. If the WrappedDriver
        /// is an <see cref="EventFiringWebDriver"/> event listeners will be
        /// added to the <see cref="EventFiringWebDriver.Navigated"/> event
        /// which will call <see cref="Dispose()"/> on this instance.
        /// 
        /// NOTE:
        /// If overriding don't forget to either call base.Load() or make sure
        /// the <see cref="Uri"/> and the <see cref="WindowHandle"/> are
        /// assigned to.
        /// </summary>
        /// <returns>
        /// A reference to this
        /// <see cref="T:OpenQA.Selenium.Support.UI.ILoadableComponent" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if this instance has already been loaded.
        /// </exception>
        /// <exception cref="UriFormatException">
        /// Thrown if the current url doens't match the Route.
        /// </exception>
        public virtual ILoadableComponent Load()
        {
            // Verify not disposed.
            if (disposedValue)
                throw new ObjectDisposedException(nameof(PageObject));

            // Check if the Route matches the current url.
            if (null == Route.Match(BaseUri, new Uri(WrappedDriver.Url)))
            {
                throw new UriFormatException("The current url failed to match the " +
                    "Route.");
            }

            // Assign event listeners if the driver is an EventFiringWebDriver.
            if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver
                && !assignedEventListeners)
            {
                eventFiringWebDriver.Navigated += OnNavigation;
                assignedEventListeners = true;
            }

            // Assign body element which will be used for checking if the page
            // is stale.
            bodyElement = WrappedDriver.FindElement(By.TagName("body"));

            // Assign the window handle.
            WindowHandle = WrappedDriver.CurrentWindowHandle;

            // Assign the Uri.
            Uri = new Uri(WrappedDriver.Url);

            return this;
        }

        /// <summary>
        /// Checks if the current url matches the <see cref="BaseUri"/> and the
        /// <see cref="Route"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the current url matches this instances
        /// <see cref="BaseUri"/> and <see cref="Route"/>; otherwise
        /// <c>false</c>.
        /// </returns>
        protected virtual bool OnValidUrl()
        {
            var currentUrl = new Uri(WrappedDriver.Url);
            var templateMatch = Route.Match(BaseUri, currentUrl);

            return templateMatch != null;
        }

        private void OnNavigation(object sender,
            WebDriverNavigationEventArgs eventArgs)
        {
            if (IsStale())
                Dispose();
        }

        #region IDisposable Support

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver
                        && assignedEventListeners)
                    {
                        eventFiringWebDriver.Navigated -= OnNavigation;
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }
}
