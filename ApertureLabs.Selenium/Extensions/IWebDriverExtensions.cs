using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Contains extensions for IWebDriver.
    /// </summary>
    public static class IWebDriverExtensions
    {
        /// <summary>
        /// Shorthand for creating a new WebDriverWait object.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static WebDriverWait Wait(this IWebDriver driver,
            TimeSpan timeSpan)
        {
            return new WebDriverWait(driver, timeSpan);
        }

        /// <summary>
        /// Shorthand for creating a new WebDriverWait object which ignores all
        /// exceptions passed in to it.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeSpan"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        public static WebDriverWait Wait(this IWebDriver driver,
            TimeSpan timeSpan,
            IEnumerable<Type> ignore)
        {
            var wait = new WebDriverWait(driver, timeSpan);
            wait.IgnoreExceptionTypes(ignore.ToArray());

            return wait;
        }

        /// <summary>
        /// Shorthand selector for FindElements(By.CssSelector(...));
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cssSelector"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(this IWebDriver driver,
            string cssSelector,
            TimeSpan timeout = default)
        {
            if (timeout == default)
            {
                return driver.FindElements(By.CssSelector(cssSelector));
            }
            else
            {
                IReadOnlyList<IWebElement> elements = null;
                driver.Wait(timeout).Until(d =>
                {
                    elements = d.FindElements(By.CssSelector(cssSelector));
                    return elements.Count > 0;
                });

                return elements;
            }
        }

        /// <summary>
        /// Shorthand selector for FindElements(by);
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(this IWebDriver driver,
            By by)
        {
            return driver.FindElements(by);
        }

        /// <summary>
        /// Retrieves the size of the current viewport.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static Size GetCurrentViewportDimensions(this IWebDriver driver)
        {
            var innerWidth = driver.ExecuteJavaScript<int>("window.innerWidth");
            var innerHeight = driver.ExecuteJavaScript<int>("window.innerHeight");

            return new Size
            {
                Width = innerWidth,
                Height = innerHeight
            };
        }

        /// <summary>
        /// Shorthand for <code>new Actions(driver);</code>. Only exists
        /// to help make the Actions class more apparent.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static Actions CreateActions(this IWebDriver driver)
        {
            return new Actions(driver);
        }

        /// <summary>
        /// Detects if jQuery is defined on a page.
        /// </summary>
        public static bool PageHasJQuery(this IWebDriver driver)
        {
            var script = "(function() { return jQuery == null })()";
            return driver.ExecuteJavaScript<bool>(script);
        }

        /// <summary>
        /// Returns a javascript executor from the webdriver. Will throw an
        /// exception if the driver doesn't support the interface.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static IJavaScriptExecutor JavaScriptExecutor(this IWebDriver driver)
        {
            if (driver is IJavaScriptExecutor jsExe)
            {
                return jsExe;
            }
            else
            {
                throw new NotImplementedException("The driver doesn't " +
                    "implement IJavaScriptExecutor.");
            }
        }

        /// <summary>
        /// Waits for a document level event matching the eventName to occur.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="eventName"></param>
        public static void WaitForEvent(this IWebDriver driver,
            string eventName)
        {
            var script =
                "var callback = arguments[arguments.length - 1];" +
                "var evtList = document.addEventListener('" + eventName + "'," +
                    "function (e) {" +
                        "document.removeEventListener('" + eventName + "', evtList);" +
                        "callback();" +
                    "})";

            driver
                .JavaScriptExecutor()
                .ExecuteAsyncScript(script);
        }

        /// <summary>
        /// Waits for the first event to be emitted by the document.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="eventNames"></param>
        /// <param name="timeout">Defaults to 30 seconds.</param>
        public static void WaitForAnyEvent(this IWebDriver driver,
            IEnumerable<string> eventNames,
            TimeSpan? timeout = null)
        {
            if (!eventNames?.Any() ?? true)
                throw new ArgumentException(nameof(eventNames));

            var script =
                "var callback = arguments[arguments.length - 1];" +
                "var eventListeners = [];" +
                "var evtListener = null;";

            foreach (var eventName in eventNames)
            {
                script +=
                    $"evtListener = document.addEventListener('" +
                        $"{eventName}'," +
                        $"function (e) {{" +
                            $"for (var i = 0; i < eventListeners.length; i++) {{" +
                                $"var handler = eventListeners[i];" +
                                $"document.removeEventListener(" +
                                    $"handler.event," +
                                    $"handler.handler);" +
                            $"}}" +
                            $"callback();" +
                        $"}});" +
                    $"eventListeners.push({{" +
                        $"handler: evtListener," +
                        $"event: {eventName}" +
                    $"}})";
            }

            driver.Wait(timeout ?? TimeSpan.FromSeconds(30))
                .Until(d => d.JavaScriptExecutor().ExecuteAsyncScript(script));
        }

        /// <summary>
        /// Waits for all events to be emitted by the document.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="eventNames"></param>
        /// <param name="timeout">Defaults to 30 seconds.</param>
        public static void WaitForAllEvents(this IWebDriver driver,
            IEnumerable<string> eventNames,
            TimeSpan? timeout = null)
        {
            if (!eventNames?.Any() ?? true)
                throw new ArgumentException(nameof(eventNames));

            var script =
                "var callback = arguments[arguments.length - 1];" +
                "var eventListeners = [];" +
                "var evtListener = null;";

            foreach (var eventName in eventNames)
            {
                script +=
                    $"evtListener = document.addEventListener('" +
                        $"{eventName}'," +
                        $"function (e) {{" +
                            $"for (var i = 0; i < eventListeners.length; i++) {{" +
                                $"var handler = eventListeners[i];" +
                                $"if (handler.event === '{eventName}') {{" +
                                    $"document.removeEventListener(" +
                                        $"handler.event," +
                                        $"handler.handler);" +
                                    $"eventListeners.remove(handler);" +
                                $"}}" +
                            $"}}" +
                            $"if (eventListeners.length === 0) {{" +
                                $"callback();" +
                            $"}}" +
                        $"}});" +
                    $"eventListeners.push({{" +
                        $"handler: evtListener," +
                        $"event: {eventName}" +
                    $"}});";
            }

            driver.Wait(timeout ?? TimeSpan.FromSeconds(30))
                .Until(d => d.JavaScriptExecutor().ExecuteAsyncScript(script));
        }
    }
}
