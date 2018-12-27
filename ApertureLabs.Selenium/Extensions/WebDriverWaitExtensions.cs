﻿using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Extensions for WebDriverWait.
    /// </summary>
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
        /// <param name="by"></param>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the element exists.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static bool Exists(
            this WebDriverWait wait,
            By by)
        {
            try
            {
                return wait.Until(driver =>
                {
                    return driver.FindElements(by).Any();
                });
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
                return false;
            }
        }

        /// <summary>
        /// Waits until the element is stale.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="element"></param>
        public static void UntilStale(this WebDriverWait wait, IWebElement element)
        {
            wait.Until(driver =>
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
            });
        }

        /// <summary>
        /// Identical to wait, but returns the IWait object instead of the
        /// wait result.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool TrySequentialWait(this IWait<IWebDriver> wait,
            out Exception exception,
            params Func<IWebDriver, bool>[] conditions)
        {
            if (wait == null)
                throw new ArgumentNullException(nameof(wait));

            exception = null;

            foreach (var condition in conditions)
            {
                try
                {
                    wait.Until(condition);
                }
                catch (Exception e)
                {
                    // Assign exception.
                    exception = e;
                    break;
                }
            }

            return exception == null;
        }
    }
}
