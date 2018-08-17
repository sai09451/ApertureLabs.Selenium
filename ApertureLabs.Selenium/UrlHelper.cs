using ApertureLabs.Selenium.WebDriver;
using System;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Contains methods for checking if the 
    /// </summary>
    public class UrlHelper
    {
        #region Fields

        protected readonly WebDriverV2 driver;
        protected Uri url;

        #endregion

        #region Constructor

        public UrlHelper(string url, WebDriverV2 driver)
            : this(new Uri(url), driver)
        { }

        public UrlHelper(Uri url, WebDriverV2 driver)
        {
            this.url = url;
            this.driver = driver;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Checks if the url from the IWebDriver matches this url.
        /// </summary>
        /// <exception cref="Exception">
        /// Will throw an exception if not on the correct tab/window.
        /// </exception>
        public bool AlreadyOnUrl
        {
            get
            {
                AssertOnCorrectWindowHandle();

                var c_uri = new Uri(WebDriver..Url);
                var d_uri = new Uri(desiredUrl);

                return (Uri.Compare(
                    c_uri,
                    d_uri,
                    UriComponents.Path,
                    UriFormat.SafeUnescaped,
                    StringComparison.CurrentCultureIgnoreCase) == 0);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Waits until the url of the page changes in the TimeSpan which
        /// defaults to 30 seconds if left null.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeout"></param>
        public void WaitUntilUrlChanges(TimeSpan? timeout = null)
        {
            Utils.AssertWaitTime(ref timeout,
                driver.DefaultTimeout,
                driver.WebDriver);

            var curl = driver.Url;

            driver.WaitUntil((_driver) =>
            {
                if (_driver.Url == curl)
                    return false;
                else
                    return true;
            });
        }

        #endregion

        #region Conversion Operators

        /// <summary>
        /// Implicit conversion operator to a string.
        /// </summary>
        /// <param name="url"></param>
        public static implicit operator string(UrlHelper url)
        {
            return url.url.AbsolutePath;
        }

        //public static implicit operator Url(string url)
        //{
        //    return new Url(url, "how... do i get this?");
        //}

        /// <summary>
        /// Implicit conversion operator to a Uri.
        /// </summary>
        /// <param name="url"></param>
        public static implicit operator Uri(UrlHelper url)
        {
            return url.url;
        }

        #endregion
    }
}
