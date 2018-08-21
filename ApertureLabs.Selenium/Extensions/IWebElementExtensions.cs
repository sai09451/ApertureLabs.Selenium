using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Extensions
{
    public static class IWebElementExtensions
    {
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
            string jsScript = "return arguments[0].parentNode;";
            return element.GetDriver()
                .ExecuteJavaScript<IWebElement>(jsScript, element);
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
    }
}
