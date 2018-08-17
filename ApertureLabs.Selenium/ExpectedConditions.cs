using ApertureLabs.Selenium.WebDriver;
using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium
{
    public static class ExpectedConditions
    {
        public static Func<IWebDriverV2, IList<IWebElement>> ElementsExist(string cssQuery)
        {
            return (driver) =>
            {
                return driver.Select(cssQuery);
            };
        }
    }
}
