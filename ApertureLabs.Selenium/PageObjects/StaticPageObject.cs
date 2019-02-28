﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Default implementation of IPageObject. Used to represent web-pages
    /// which have urls that don't have any variables in them. Use the
    /// <see cref="ParameterPageObject" /> class for page objects whose urls
    /// do have parameters.
    /// </summary>
    public abstract class StaticPageObject : IStaticPageObject, IDisposable
    {
        #region Fields

        private readonly Regex uriMatcher;

        private bool assignedEventListeners;
        private bool disposedValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticPageObject"/> class.
        /// If passing in an EventFiringWebDriver event listeners will be
        /// added.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="uriMatcher">The URI matcher.</param>
        public StaticPageObject(IWebDriver driver, string uriMatcher)
        {
            WrappedDriver = driver
                ?? throw new ArgumentNullException(nameof(driver));
            Route = uriMatcher
                ?? throw new ArgumentNullException(nameof(uriMatcher));

            assignedEventListeners = false;
            disposedValue = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the OpenQA.Selenium.IWebDriver used to find this element.
        /// </summary>
        public IWebDriver WrappedDriver { get; private set; }

        /// <summary>
        /// Gets the URI. Should be null until <c>Load</c> is called where it
        /// will be set to the Url the <see cref="T:OpenQA.Selenium.IWebDriver" />
        /// is on.
        /// </summary>
        public Uri Uri { get; protected set; }

        /// <summary>
        /// Used to match the Uri of the webpage. Is used to create a Regex
        /// object to verify the url of the page when calling <c>Load</c>.
        /// NOTE: All meta-sequences should be in groups (preferrably named
        /// groups).
        /// </summary>
        public string Route { get; protected set; }

        /// <summary>
        /// Gets the window handle the page was originally loaded on.
        /// </summary>
        public string WindowHandle { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="IPageObject" />, is
        /// equal to this instance.
        /// </summary>
        /// <param name="pageObject">The page object.</param>
        /// <returns></returns>
        public virtual bool Equals(IPageObject pageObject)
        {
            if (pageObject == null)
                throw new ArgumentNullException(nameof(pageObject));

            return GetHashCode() == pageObject.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is
        /// equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with
        /// this instance.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is
        ///   equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IPageObject pageObject)
                return Equals(pageObject);
            else
                return base.Equals(obj);
        }

        /// <summary>
        /// By default will to see if the pages original window handle still
        /// exists and that windows url matches the Uri.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsStale()
        {
            // Check if this page objects window handle isn't null and that
            // the window handle still exists.
            if (!String.IsNullOrEmpty(WindowHandle)
                && !WrappedDriver.WindowHandles.Contains(WindowHandle))
            {
                // Store the current window handle.
                var originalWindowHandle = WrappedDriver.CurrentWindowHandle;

                // If the uri is null, assume true.
                if (Uri == null || String.IsNullOrEmpty(Route))
                    return true;

                try
                {
                    // Switch to this pages window handle.
                    WrappedDriver.SwitchTo().Window(WindowHandle);

                    // Check if the url matches this pages url.
                    var currentUri = new Uri(WrappedDriver.Url);

                    // Ignore the query string, only concerned by the path.
                    var rawUrl = currentUri.GetLeftPart(UriPartial.Path);

                    var isOnSameUrl = !String.Equals(
                        rawUrl,
                        Uri.ToString(),
                        StringComparison.OrdinalIgnoreCase);

                    if (isOnSameUrl)
                        return true;
                }
                finally
                {
                    // Switch back to the original window handle.
                    WrappedDriver.SwitchTo().Window(originalWindowHandle);
                }
            }

            return false;
        }

        /// <summary>
        /// If overridding this don't forget to call base.Load().
        /// NOTE: Will navigate to the pages url if the current drivers url
        /// is empty.
        /// </summary>
        /// <remarks>
        /// If the driver is an EventFiringWebDriver an event listener will
        /// be added to the 'Navigated' event and uses the url to determine
        /// if the page is 'stale'.
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException">
        /// Occurs when trying to use after this instance has been
        /// disposed.
        /// </exception>
        /// <exception cref="LoadableComponentException">
        /// Thrown when calling <code>Load()</code> and not on the correct
        /// url.
        /// </exception>
        public virtual ILoadableComponent Load()
        {
            if (disposedValue)
                throw new ObjectDisposedException(nameof(StaticPageObject));

            if (WrappedDriver is EventFiringWebDriver eventFiringWebDriver
                && !assignedEventListeners)
            {
                eventFiringWebDriver.Navigated += OnNavigation;
                assignedEventListeners = true;
            }

            // Navigate to this pages Uri if the current url is blank.
            var shouldNavigate = Uri != null || !UriMatcherContainerWildCards();

            if (shouldNavigate)
            {
                // Verify the Uri isn't null and the driver isn't already on
                // the url.
                var canUseUri = !String.Equals(
                    Uri?.ToString() ?? "",
                    WrappedDriver.Url,
                    StringComparison.OrdinalIgnoreCase);

                var canUseUriMatcher = !canUseUri
                    && !UriMatcherContainerWildCards();

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

            // Assign the window handle and Uri.
            WindowHandle = WrappedDriver.CurrentWindowHandle;
            Uri = new Uri(WrappedDriver.Url);

            return this;
        }

        public ILoadableComponent Load(bool firstNavigateToUrl)
        {
            if (!OnCorrectUrl())
                WrappedDriver.Navigate().GoToUrl(Route);

            return Load();
        }

        /// <summary>
        /// Returns a hash code for this instance. Only uses the WrappedDriver,
        /// Uri, and the WindowHandle to generate the hash code.
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
                + EqualityComparer<string>.Default.GetHashCode(Route);
            hashCode = hashCode
                * -1521134295
                + EqualityComparer<string>.Default.GetHashCode(WindowHandle);
            return hashCode;
        }

        protected bool UriMatcherContainerWildCards()
        {
            return Route.Contains("*")
                || Route.Contains("{")
                || Route.Contains("}");
        }

        protected bool OnCorrectUrl()
        {
            var url = WrappedDriver.Url;
            var instanceUrl = Uri?.ToString() ?? Route;

            return String.Equals(url,
                instanceUrl,
                StringComparison.OrdinalIgnoreCase);
        }

        private void OnNavigation(object sender, WebDriverNavigationEventArgs eventArgs)
        {
            if (IsStale())
            {
                Dispose();
            }
        }

        private bool CanNavigateToUri()
        {
            return false;
        }

        private bool CanNavigateToUriMatcher()
        {
            return false;
        }

        #region IDisposable Support
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        #endregion
    }
}