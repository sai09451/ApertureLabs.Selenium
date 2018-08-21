using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium
{
    public static class ExpectedConditions
    {
        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsExist(By by)
        {
            return (driver) =>
            {
                return driver.FindElements(by);
            };
        }

        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsAreVisible(By by)
        {
            return driver =>
            {
                var elements = driver.FindElements(by);
                var allVisible = elements.All(element => element.Displayed);

                return allVisible ? elements : null;
            };
        }

        public static Func<IWebDriver, IReadOnlyList<IWebElement>> ElementsAreClickable(By by)
        {
            return driver =>
            {
                var elements = driver.FindElements(by);
                var viewPageDimensions = driver.Manage().Window;
                var allClickable = elements.All(element => element.Displayed
                    && element.Location);

                return allClickable ? elements : null;
            };
        }
    }
}
