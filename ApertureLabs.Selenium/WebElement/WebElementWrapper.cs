using ApertureLabs.Selenium;
using ApertureLabs.Selenium.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ApertureLabs.Selenium.WebElement
{
    /// <summary>
    /// Any class that inherits from this class MUST have a constructor that
    /// has a sole argument which takes an IWebElement.
    /// </summary>
    public class WebElementWrapper : ICssQueryContext
    {
        #region Fields

        protected readonly WebDriverWrapper driver;

        #endregion

        #region Constructor

        public WebElementWrapper(IWebElement element, WebDriverWrapper driver)
        {
            this.driver = driver;
            this.WebElement = element;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the wrapped element.
        /// </summary>
        public IWebElement WebElement { get; private set; }

        /// <summary>
        /// Utilities for working with the text in the element.
        /// </summary>
        public TextHelper Text { get; private set; }

        /// <summary>
        /// Returns the tag name of the element.
        /// </summary>
        public string TagName { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Moves the mouse to an element and hovers over it for the provided
        /// TimeSpan.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="timespan"></param>
        public void Hover(TimeSpan timespan)
        {
            if (!WebElement.Displayed)
                throw new ElementNotVisibleException();

            var actions = driver.CreateAction();
            actions.MoveToElement(WebElement).Perform();
            Thread.Sleep(timespan);
        }

        /// <summary>
        /// Similar to the native Selenium SendKeys(...) method but also
        /// waits until the element is ready (exists, visibile, and clickable)
        /// before sending keys to it.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="keys"></param>
        /// <param name="wait"></param>
        public void SendKeys(string selector, string keys, TimeSpan? wait = null)
        {
            Utils.AssertWaitTime(ref wait, driver.DefaultWait, driver.WebDriver);

            driver.WebDriver.WaitUntilReady(selector, wait).SendKeys(keys);
        }

        /// <summary>
        /// Checks to see if the element contains anything matching the
        /// cssSelector.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public bool Contains(IWebElement element, string cssSelector)
        {
            // Escape any double quotes in the selector
            cssSelector = cssSelector.Replace("\"", "\\\"");
            string jsScript = $"return (arguments[0].querySelectorAll(\"{cssSelector}\").length > 0);";
            return driver.Javascript.ExecuteJs<bool>(jsScript, element);
        }

        /// <summary>
        /// Selects only direct child elements of this element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public IList<IWebElement> Children(IWebElement element)
        {
            string jsScript = "return arguments[0].children;";
            return driver.Javascript
                .ExecuteJs<IList<IWebElement>>(jsScript, element);
        }

        public IList<WebElementWrapper> Select(string cssSelector, TimeSpan> wait = null)
        {
            
        }

        public T As<T>() where T:WebElementWrapper,new()
        {
            return Activator.CreateInstance(typeof(T), WebElement) as T;
        }

        /// <summary>
        /// Retrieves the webdriver from an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebDriver GetWebDriver(IWebElement element)
        {
            return ((IWrapsDriver)element).WrappedDriver;
        }
        #endregion
    }
}
