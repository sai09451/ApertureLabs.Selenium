using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace ApertureLabs.Selenium.WebDriver
{
    public class WebDriverWrapper : ICssQueryContext
    {
        #region Fields
        private readonly WebDriverWait wait;
        private TimeSpan _defaultWait;
        #endregion

        #region Constructor

        public WebDriverWrapper(IWebDriver driver,
            TimeSpan? defaultWait = null)
        {
            WebDriver = driver;
            DefaultWait = defaultWait ?? TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, DefaultWait);
            Javascript = new JavascriptHelper(this);
            Tabs = new TabHelper(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the webdriver.
        /// </summary>
        public IWebDriver WebDriver { get; private set; }

        /// <summary>
        /// Contains utilities for running js scripts.
        /// </summary>
        public JavascriptHelper Javascript { get; private set; }

        public TabHelper Tabs { get; private set; }

        public TimeSpan DefaultWait
        {
            get
            {
                return _defaultWait;
            }
            set
            {

            }
        }

        #endregion

        #region Methods

        private void AssertWaitTime(ref TimeSpan? wait)
        {
            WebDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

            if (wait == null)
                wait = DefaultWait;
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

            height = height ?? WebDriver.Manage().Window.Size.Height;

            WebDriver.Manage().Window.Size = new Size(width, height.Value);
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
                size.Height = WebDriver.Manage().Window.Size.Height;

            WebDriver.Manage().Window.Size = size;
        }

        /// <summary>
        /// Shortcut for refreshing the page.
        /// </summary>
        /// <param name="driver"></param>
        public void RefreshPage()
        {
            WebDriver.Navigate().GoToUrl(WebDriver.Url);
        }

        /// <summary>
        /// Returns a new Action.
        /// </summary>
        /// <returns></returns>
        public Actions CreateAction()
        {
            return new Actions(WebDriver);
        }

        /// <summary>
        /// Waits until the url of the page changes in the TimeSpan which
        /// defaults to 30 seconds if left null.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeout"></param>
        public void WaitUntilUrlChanges(TimeSpan? timeout = null)
        {
            AssertWaitTime(ref timeout);

            var w = new WebDriverWait(WebDriver, timeout.Value);
            var curl = WebDriver.Url;

            w.Until((_driver) =>
            {
                if (_driver.Url == curl)
                    return false;
                else
                    return true;
            });
        }

        /// <summary>
        /// Searches for the element matching the css selector for the duration
        /// of the TimeSpan which defaults to 30 seconds if left null.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cssSelector"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public IList<WebDriverWrapper> Select(string cssSelector, TimeSpan? wait = null)
        {
            AssertWaitTime(ref wait);

            var w = new WebDriverWait(WebDriver, wait.GetValueOrDefault());
            w.Until(ExpectedConditions.ElementExists(By.CssSelector(cssSelector)));
            return WebDriver.FindElements(By.CssSelector(cssSelector))
                .Select(element => new WebElementWrapper(element, this))
                .ToList() as IList<WebDriverWrapper>;
        }
        #endregion
    }
}
