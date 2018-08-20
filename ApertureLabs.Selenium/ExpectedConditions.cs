using ApertureLabs.Selenium.WebDriver;
using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium
{
    public static class ExpectedConditions
    {
        public static Func<IWebDriverV2, IList<IWebElementV2>> ElementsExist(string cssQuery)
        {
            return (driver) =>
            {
                return driver.Select(cssQuery, TimeSpan.FromSeconds(0));
            };
        }

        public static Func<IWebDriverV2, IList<IWebElementV2>> ElementsAreVisible(string cssQuery)
        {
            return (driver) =>
            {
                var elements = driver.Select(cssQuery, TimeSpan.FromSeconds(0));
                var result = elements.All(element => element);
                return result ? elements : null;
            };
        }

        public static Func<IWebDriverV2, IList<IWebElementV2>> ElementsAreClickable(
            string cssQuery)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Waits until elements found by the sector exists, are visible, and
        /// are clickable then returns said elements.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="cssSelector"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static Func<IWebDriverV2, IList<IWebElementV2>> WaitUntilReady(
            string cssSelector,
            TimeSpan? wait = null)
        {
            Utils.AssertWaitTime(ref wait,
                driver.DefaultTimeout,
                driver.GetNativeWebDriver());

            IList<IWebElementV2> elements = null;
            var previousTimeout = driver.DefaultTimeout;
            var expiration = DateTime.Now + wait.GetValueOrDefault();

            do
            {
                if (!Utils.TimeLeft(expiration, out TimeSpan remaining))
                {
                    break;
                }

                driver.DefaultTimeout = remaining;
                driver.WaitUntil(ExpectedConditions.ElementsExist(cssSelector));

                if (!Utils.TimeLeft(expiration, out remaining))
                {
                    break;
                }

                driver.DefaultTimeout = remaining;
                driver.WaitUntil(ExpectedConditions.ElementsAreVisible(cssSelector));

                if (!Utils.TimeLeft(expiration, out remaining))
                {
                    break;
                }

                driver.DefaultTimeout = remaining;
                elements = driver.Select(cssSelector, remaining);
                w.Timeout = remaining;
                elements = w.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(cssSelector)));
            } while (false);

            return elements;
        }
    }
}
