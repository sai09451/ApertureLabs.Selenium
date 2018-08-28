using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Extensions
{
    public static class WebDriverWaitExtensions
    {
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

        /// <summary>
        /// Shorthand selector for wait.Until(ExpectedConditions.ElementsExist(
        /// by));
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public static IReadOnlyList<IWebElement> Select(
            this WebDriverWait wait,
            By by)
        {
            return wait.Until(driver => driver.FindElements(by));
        }

        /// <summary>
        /// Returns true if the element exists.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
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
    }
}
