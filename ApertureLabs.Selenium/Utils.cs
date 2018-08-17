using OpenQA.Selenium;
using System;

namespace ApertureLabs.Selenium
{
    internal static class Utils
    {
        internal static void AssertWaitTime(ref TimeSpan? wait,
            TimeSpan defaultWait,
            IWebDriver driver)
        {
            if (!wait.HasValue)
            {
                wait = defaultWait;
            }

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);
        }
    }
}
