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
        /// Shorthand selector for FindElements(By.CssSelector
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
        /// Shorthand selector for wait.Until(ExpectedConditions.ElementsExist(
        /// By.CssSelector('cssSelector')));
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(
            this WebDriverWait wait,
            string cssSelector)
        {
            return wait.Until((driver) =>
            {
                return driver.FindElements(By.CssSelector(cssSelector));
            });
        }

        public static bool Exists(
            this WebDriverWait wait,
            string cssSelector)
        {
            try
            {
                return wait.Until(driver =>
                {
                    return driver.FindElements(By.CssSelector(cssSelector))
                        .Any();
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            catch (Exception exc)
            {
                return false;
            }
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
