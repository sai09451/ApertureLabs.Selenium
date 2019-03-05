using ApertureLabs.Selenium.Js;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var el = element.UnWrapEventFiringWebElement();
            return el.GetDriver()
                .ExecuteJavaScript<IList<IWebElement>>(jsScript, el);
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
        /// This is a fix for trying to pass elements of the nested private
        /// class EventFiringWebElement of
        /// OpenQA.Selenium.Support.Events.EventFiringWebDriver.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebElement UnWrapEventFiringWebElement(
            this IWebElement element)
        {
            IWebElement el = element;

            // EventFiringWebElements don't implemented IWrapsDriver but
            // RemoteElements do, so just keep unwrapping until the
            // RemoteWebElement is reached.
            while (el is IWrapsElement
                && !(el is IWrapsDriver)
                && !(el is RemoteWebElement))
            {
                var e = el as IWrapsElement;
                el = e.WrappedElement;
            }

            return el;
        }

        /// <summary>
        /// Retrieves the IWebDriver from an IWebElement.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IWebDriver GetDriver(this IWebElement element)
        {
            var driver = default(IWebDriver);
            var el = element.UnWrapEventFiringWebElement();

            driver = (el as IWrapsDriver)?.WrappedDriver;

            if (driver == null)
            {
                throw new NotImplementedException("Failed to cast the " +
                    "wrapped element to an IWrapsDriver or an IWrapsElement " +
                    "interface.");
            }

            return driver;
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
            var el = element.UnWrapEventFiringWebElement();

            var result = el.GetDriver()
                .JavaScriptExecutor()
                .ExecuteScript(jsScript, el);

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
            var el = element.UnWrapEventFiringWebElement();

            if (String.IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            var waiter = new PromiseBody(el.GetDriver())
            {
                Arguments = new[] { new JavaScriptValue(el) },
                Script =
                    "var el = {args}[0];" +
                    "var callback = {resolve};" +
                    "var eventListener = el.addEventListener('" + eventName + "'," +
                        "function(e) {" +
                            "el.removeEventListener('" + eventName + "', eventListener);" +
                            "callback();" +
                        "});"
            };

            waiter.Execute(el.GetDriver().JavaScriptExecutor());

            return waiter;
        }

        /// <summary>
        /// Waits for an event to occur on the element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="eventName"></param>
        public static void WaitForEvent(this IWebElement element, string eventName)
        {
            var el = element.UnWrapEventFiringWebElement();
            var js =
                "var el = arguments[0];" +
                "var callback = arguments[arguments.length - 1];" +
                "var eventListener = el.addEventListener('" + eventName + "'," +
                    "function(e) {" +
                        "el.removeEventListener('" + eventName + "', eventListener);" +
                        "callback();" +
                    "});";

            var d = el.GetDriver();
            d.JavaScriptExecutor().ExecuteAsyncScript(js, el);
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
            var el = element.UnWrapEventFiringWebElement();

            if (!eventNames?.Any() ?? true)
            {
                throw new ArgumentException($"{nameof(eventNames)} was null " +
                    $"or empty.");
            }

            var tasks = eventNames.Select(e =>
            {
                var evtName = e;
                return new Task(() => el.WaitForEvent(evtName));
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
            var el = element.UnWrapEventFiringWebElement();

            if (!eventNames?.Any() ?? true)
            {
                throw new ArgumentException($"{nameof(eventNames)} was null " +
                    $"or empty.");
            }

            var tasks = eventNames.Select(e =>
            {
                var evtName = e;
                return new Task(() => el.WaitForEvent(evtName));
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
            var el = element.UnWrapEventFiringWebElement();
            var value = default(string);
            var driver = el.GetDriver();

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

                value = driver
                    .JavaScriptExecutor()
                    .ExecuteScript(script, el)
                    .ToString();
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
            if (element == null)
                throw new ArgumentNullException(nameof(element));

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

            var el = element.UnWrapEventFiringWebElement();

            var indexStr = el.GetDriver()
                .JavaScriptExecutor()
                .ExecuteScript(script, el)
                .ToString();

            var index = Int32.Parse(indexStr, CultureInfo.CurrentCulture);

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
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var el = element.UnWrapEventFiringWebElement();
            var driver = el.GetDriver();

            return driver
                .FindElements(selector)
                .Any(e => e.Equals(el));
        }
    }
}
