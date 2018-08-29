using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ApertureLabs.Selenium.Extensions
{
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
        /// <param name=""></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(this IWebDriver driver,
            string cssSelector,
            TimeSpan timeout = default(TimeSpan))
        {
            if (timeout == default(TimeSpan))
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
    }
}
