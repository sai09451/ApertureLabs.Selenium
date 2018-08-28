using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System.Collections.Generic;
using System.Drawing;

namespace ApertureLabs.Selenium.Extensions
{
    public static class IWebDriverExtensions
    {
        /// <summary>
        /// Shorthand selector for FindElements(By.CssSelector(...));
        /// </summary>
        /// <param name="driver"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(this IWebDriver driver,
            string cssSelector)
        {
            return driver.FindElements(By.CssSelector(cssSelector));
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
