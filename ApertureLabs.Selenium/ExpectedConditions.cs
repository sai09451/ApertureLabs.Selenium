using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Commonly used conditions.
    /// </summary>
    public static class ExpectedConditions
    {
        /// <summary>
        /// Waits for at least one element identified by the selector to exist.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsExist(By by)
        {
            return (driver) =>
            {
                return driver.FindElements(by);
            };
        }

        /// <summary>
        /// Waits until all elements identified by the selector are visible.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsAreVisible(By by)
        {
            return driver =>
            {
                var elements = driver.FindElements(by);
                var allVisible = elements.All(element => element.Displayed);

                return allVisible ? elements : null;
            };
        }

        /// <summary>
        /// Waits until all elements identified by the selector are clickable.
        /// </summary>
        /// <param name="by"></param>
        /// <returns></returns>
        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsAreClickable(By by)
        {
            return driver =>
            {
                var elements = driver.FindElements(by);
                var viewPageDimensions = driver.Manage().Window;
                //var allClickable = elements.All(element => element.Displayed
                //    && element.Location);
                var allClickable = elements.All(element => element.Displayed);

                return allClickable ? elements : null;
            };
        }
    }
}
