using OpenQA.Selenium;
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
        /// Waits until the page reloads. This is accomplished by first
        /// locating the root html element, polling it until it becomes stale,
        /// and then waiting until the document.readyState is valid.
        /// </summary>
        /// <param name="wait"></param>
        /// <param name="readyStates">
        /// Defaults to "complete". These values are case sensitive.
        /// </param>
        public static void UntilPageReloads(this WebDriverWait wait,
            IEnumerable<string> readyStates = null)
        {
            if (readyStates == null)
                readyStates = new List<string> { "complete" };

            IReadOnlyList<IWebElement> htmlEls = null;
            var by = By.TagName("html");
            var js = "return " + string.Join(" || ", readyStates.Select(s => $"(document.readyState === {s})"));

            if (wait is IWrapsDriver wraps)
                htmlEls = wraps.WrappedDriver.FindElements(By.TagName("html"));
            else
                htmlEls = wait.Select(by);

            wait.Until(driver =>
            {
                try
                {
                    htmlEls.All(el => el.Enabled);
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    // Ignore
                    return true;
                }
            });

            wait.Until(driver =>
            {
                try
                {
                    return driver.ExecuteJavaScript<bool>(js);
                }
                catch
                {
                    return false;
                }
            });
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
    }
}
