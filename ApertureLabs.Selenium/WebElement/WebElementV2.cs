using ApertureLabs.Selenium;
using ApertureLabs.Selenium.WebElement;
using ApertureLabs.Selenium.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace ApertureLabs.Selenium.WebElement
{
    /// <summary>
    /// Any class that inherits from this class MUST have a constructor that
    /// has a sole argument which takes an IWebElement.
    /// </summary>
    public class WebElementV2 : IWebElementV2
    {
        #region Fields

        protected readonly IWebDriverV2 driver;

        #endregion

        #region Constructor

        public WebElementV2(IWebElement element, IWebDriverV2 driver)
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

        public bool TryScrollElementToCenterView()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Waits until elements found by the sector exists, are visible, and
        /// are clickable then returns said elements.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cssSelector"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public IWebElementV2 WaitUntilReady(string cssSelector,
            TimeSpan? wait = null)
        {
            throw new NotImplementedException();

            //Utils.AssertWaitTime(ref wait,
            //    driver.DefaultTimeout,
            //    driver.GetNativeWebDriver());

            //IList<IWebElementV2> elements = null;
            //var previousTimeout = driver.DefaultTimeout;
            //var expiration = DateTime.Now + wait.GetValueOrDefault();

            //do
            //{
            //    if (!Utils.TimeLeft(expiration, out TimeSpan remaining))
            //    {
            //        break;
            //    }

            //    driver.DefaultTimeout = remaining;
            //    driver.WaitUntil(ExpectedConditions.ElementsExist(cssSelector));

            //    if (!Utils.TimeLeft(expiration, out remaining))
            //    {
            //        break;
            //    }

            //    driver.DefaultTimeout = remaining;
            //    driver.WaitUntil(ExpectedConditions.ElementsAreVisible(cssSelector));

            //    if (!Utils.TimeLeft(expiration, out remaining))
            //    {
            //        break;
            //    }

            //    driver.DefaultTimeout = remaining;
            //    elements = driver.Select(cssSelector, remaining);
            //    w.Timeout = remaining;
            //    elements = w.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(cssSelector)));
            //} while (false);

            //return elements;
        }

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
            Utils.AssertWaitTime(ref wait,
                driver.DefaultTimeout,
                driver.GetNativeWebDriver());

            var elements = driver.WaitUntil(
                ExpectedConditions.WaitUntilReady(selector),
                wait);
            elements.First().WebElement.SendKeys(keys);
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

        public T As<T>() where T:WebElementV2,new()
        {
            return Activator.CreateInstance(typeof(T), WebElement) as T;
        }

        IList<IWebElementV2> ICssQueryContext.Select(string cssSelector, TimeSpan? wait)
        {
            throw new NotImplementedException();
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
