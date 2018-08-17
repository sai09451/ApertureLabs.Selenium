using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ApertureLabs.Selenium.WebDriver
{
    /// <summary>
    /// A webdriver wrapper that simplifies many operations like selecting
    /// elements, changing tabs, clearing cookies, and many other operations.
    /// </summary>
    public class WebDriverV2 : IWebDriverV2
    {
        #region Fields
        /// <summary>
        /// Contains a list of all active 
        /// </summary>
        private static IList<WebDriverV2> ActiveWebDrivers
            = new List<WebDriverV2>();
        private readonly IWebDriver webDriver;
        private readonly WebDriverWait wait;
        private TimeSpan _defaultWait;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for internal use only. Use the WebDriverFactory for
        /// getting a wrapped WebDriverWrapper.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="defaultWait"></param>
        internal WebDriverV2(IWebDriver driver,
            TimeSpan? defaultWait = null)
        {
            webDriver = driver;
            DefaultTimeout = defaultWait ?? TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, DefaultTimeout);
            Javascript = new JavascriptHelper(this);
            Tabs = new TabHelper(this);
            Url = new UrlHelper(string.Empty, this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Contains utilities for running js scripts.
        /// </summary>
        public JavascriptHelper Javascript { get; private set; }

        /// <summary>
        /// Utilities for working with tabs.
        /// </summary>
        public TabHelper Tabs { get; private set; }

        /// <summary>
        /// The default wait time for selecting elements.
        /// </summary>
        public TimeSpan DefaultTimeout
        {
            get
            {
                return _defaultWait;
            }
            set
            {
                _defaultWait = value;
                wait.Timeout = _defaultWait;
            }
        }

        /// <summary>
        /// Utilities for working with the url.
        /// </summary>
        public UrlHelper Url { get; private set; }

        #endregion

        #region Methods

        public IWebDriver GetNativeWebDriver()
        {
            return webDriver;
        }

        public TResult WaitUntil<TResult>(Func<IWebDriverV2, TResult> condition)
        {
            return wait.Until((WebDriver) =>
            {
                return condition(this);
            });
        }

        /// <summary>
        /// Sets the size of the window.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetWindowSize(int width, int? height = null)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException();

            height = height ?? webDriver.Manage().Window.Size.Height;

            webDriver.Manage().Window.Size = new Size(width, height.Value);
        }

        /// <summary>
        /// Sets the window to the size. If the height of argument is 0 then
        /// the height of the window isn't changed.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="size"></param>
        public void SetWindowSize(Size size)
        {
            if (size.Height == 0)
                size.Height = webDriver.Manage().Window.Size.Height;

            webDriver.Manage().Window.Size = size;
        }

        /// <summary>
        /// Shortcut for refreshing the page.
        /// </summary>
        /// <param name="driver"></param>
        public void RefreshPage()
        {
            webDriver.Navigate().GoToUrl(webDriver.Url);
        }

        /// <summary>
        /// Returns a new Action.
        /// </summary>
        /// <returns></returns>
        public Actions CreateAction()
        {
            return new Actions(webDriver);
        }

        /// <summary>
        /// Searches for the element matching the css selector for the duration
        /// of the TimeSpan which defaults to 30 seconds if left null.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cssSelector"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public IList<IWebElementV2> Select(string cssSelector, TimeSpan? wait)
        {
            Utils.AssertWaitTime(ref wait, DefaultTimeout, webDriver);

            WaitUntil(ExpectedConditions.ElementsExist(cssSelector));
            return webDriver.FindElements(By.CssSelector(cssSelector))
                .Select(element => new WebElementV2(element, this))
                .ToList() as IList<IWebElementV2>;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    webDriver.Close();
                    webDriver.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
        #endregion
    }
}
