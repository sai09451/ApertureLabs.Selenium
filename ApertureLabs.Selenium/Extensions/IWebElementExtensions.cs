using ApertureLabs.Selenium.Js;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Extensions for IWebElement.
    /// </summary>
    public static class IWebElementExtensions
    {
        /// <summary>
        /// Selects only direct child elements of this element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IList<IWebElement> Children(this IWebElement element)
        {
            string jsScript = "return arguments[0].children;";
            return element.GetDriver().ExecuteJavaScript<IList<IWebElement>>(
                jsScript,
                element);
        }

        /// <summary>
        /// Retrieves all classes listed on an element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<string> Classes(this IWebElement element)
        {
            return element.GetAttribute("class")?.Split(' ')
                ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Retrieves the IWebDriver from an IWebElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebDriver GetDriver(this IWebElement element)
        {
            return ((IWrapsDriver)element).WrappedDriver;
        }

        /// <summary>
        /// Will try scroll to the page until the center of the element is
        /// aligned with the center of the viewport.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool TryScrollToCenter(this IWebElement element)
        {
            try
            {
                var driver = GetDriver(element);
                var WindowHeight = driver.Manage().Window.Size.Height;
                int windowPosition = driver
                    .ExecuteJavaScript<int>("return window.scrollY");
                int windowCenterPosition = windowPosition + WindowHeight / 2;

                var elementPosition = element.Location.Y;
                var elementHeight = element.Size.Height;
                int elementCenterPosition = elementPosition + elementHeight / 2;

                int offset = elementCenterPosition - windowCenterPosition;

                driver.ExecuteJavaScript("window.scrollBy(0, " + offset + ")");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns and creates a new TextHelper for a given element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static TextHelper TextHelper(this IWebElement element)
        {
            return new TextHelper(element);
        }

        /// <summary>
        /// Returns the parent element of the element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebElement GetParentElement(this IWebElement element)
        {
            string jsScript = "return arguments[0].parentElement;";
            var result = element.GetDriver()
                .JavaScriptExecutor()
                .ExecuteScript(jsScript, element);

            return result as IWebElement;
        }

        /// <summary>
        /// Returns a random element and returns it.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IWebElement SelectRandom(
            IReadOnlyList<IWebElement> elements)
        {
            return SelectRandom(elements, out int index);
        }

        /// <summary>
        /// Returns a random element and returns it.
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static IWebElement SelectRandom(
            IReadOnlyList<IWebElement> elements,
            out int index)
        {
            if (elements.Count == 0)
            {
                throw new IndexOutOfRangeException("The collection must" +
                    " contian at least one element.");
            }

            index = new Random().Next(0, elements.Count);
            return elements[index];
        }

        /// <summary>
        /// Checks if an element is stale.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool IsStale(this IWebElement element)
        {
            try
            {
                element.GetAttribute("any");
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }

        /// <summary>
        /// Creates an activated SeleniumJavaScriptPromiseBody and returns it.
        /// NOTE: Don't call CreateScript(...) on the returned object as it has
        /// already been called.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="eventName">Name of the event.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">eventName</exception>
        public static PromiseBody GetEventWaiter(
            this IWebElement element,
            string eventName)
        {
            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var waiter = new PromiseBody(element.GetDriver())
            {
                Arguments = new object[] { element },
                Script =
                    "var el = {args}[0];" +
                    "var callback = {resolve};" +
                    "var eventListener = el.addEventListener('" + eventName + "'," +
                        "function(e) {" +
                            "el.removeEventListener('" + eventName + "', eventListener);" +
                            "callback();" +
                        "});"
            };

            waiter.Execute(element.GetDriver().JavaScriptExecutor());

            return waiter;
        }

        /// <summary>
        /// Waits for an event to occur on the element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="eventName"></param>
        public static void WaitForEvent(this IWebElement element, string eventName)
        {
            var js =
                "var el = arguments[0];" +
                "var callback = arguments[arguments.length - 1];" +
                "var eventListener = el.addEventListener('" + eventName + "'," +
                    "function(e) {" +
                        "el.removeEventListener('" + eventName + "', eventListener);" +
                        "callback();" +
                    "});";

            var d = element.GetDriver();
            d.JavaScriptExecutor().ExecuteAsyncScript(js, element);
        }

        /// <summary>
        /// Waits for the first event on the element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="eventNames"></param>
        /// <param name="timeout"></param>
        public static void WaitForAnyEvent(this IWebElement element,
            IEnumerable<string> eventNames,
            TimeSpan? timeout = null)
        {
            if (!eventNames?.Any() ?? true)
                throw new ArgumentException(nameof(eventNames));

            var tasks = eventNames.Select(e =>
            {
                var evtName = e;
                return new Task(() => element.WaitForEvent(evtName));
            });

            var result = Task.WhenAny(tasks)
                .Wait(timeout ?? TimeSpan.FromSeconds(30));

            if (!result)
                throw new TimeoutException();
        }

        /// <summary>
        /// Waits for all events to be emitted on the element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="eventNames"></param>
        /// <param name="timeout"></param>
        public static void WaitForAllEvents(this IWebElement element,
            IEnumerable<string> eventNames,
            TimeSpan? timeout = null)
        {
            if (!eventNames?.Any() ?? true)
                throw new ArgumentException(nameof(eventNames));

            var tasks = eventNames.Select(e =>
            {
                var evtName = e;
                return new Task(() => element.WaitForEvent(evtName));
            });

            var result = Task.WhenAll(tasks)
                .Wait(timeout ?? TimeSpan.FromSeconds(30));

            if (!result)
                throw new TimeoutException();
        }

        /// <summary>
        /// This exists to patch an issue with the current chromedriver not
        /// being compatible with the W3C WebDriver spec as of 05 June 2018
        /// (https://www.w3.org/TR/2018/REC-webdriver1-20180605/). This will
        /// use the native function 'GetProperty' for every driver except
        /// chromedriver which instead executes javascript to retrieve the
        /// value.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValueIfNull"></param>
        /// <returns></returns>
        public static string GetElementProperty(this IWebElement element,
            string propertyName,
            string defaultValueIfNull = null)
        {
            var value = default(string);
            var driver = element.GetDriver();

            var capabilities = driver.Capabilities();
            var isChrome = false;

            if (capabilities != null)
            {
                var browserName = (string)capabilities.GetCapability(
                    CapabilityType.BrowserName);

                if (browserName == "chrome")
                {
                    isChrome = true;
                }
            }

            if (isChrome)
            {
                // Use js to get element property.
                var script =
                    "var el = arguments[0];" +
                    "return el['" + propertyName + "'];";

                value = driver.ExecuteJavaScript<string>(script, element);
            }
            else
            {
                // Browser should support get property.
                value = element.GetProperty(propertyName);
            }

            return value ?? defaultValueIfNull;
        }

        /// <summary>
        /// Gets the position of the element relative to it's sibling elements.
        /// NOTE: Is zero based: first element of parent is zero, second is
        /// one, etc...
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static int GetIndexRelativeToSiblings(this IWebElement element)
        {
            const string script =
                    "var element = arguments[0];" +
                    "var parent = element.parentElement;" +
                    "var i = 0;" +
                    "for (var el of parent.children) {" +
                        "if (el == element) {" +
                        "   return i;" +
                        "}" +
                        "i++;" +
                    "}" +
                    "return -1";

            var indexStr = element.GetDriver()
                .JavaScriptExecutor()
                .ExecuteScript(script, element)
                .ToString();

            var index = int.Parse(indexStr);

            return index;
        }

        /// <summary>
        /// Determines whether the element matches the selector. Similar to
        /// jQuery.is(...).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="selector">The selector.</param>
        /// <returns>
        ///   <c>true</c> if the element matches the specified selector;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public static bool Is(this IWebElement element, By selector)
        {
            var driver = element.GetDriver();

            return driver.FindElements(selector).Any(e => e.Equals(element));
        }
    }
}
