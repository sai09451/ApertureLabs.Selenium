using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace ApertureLabs.Selenium.WebDriver
{
    public class WebDriverTest
    {
        public void Run(Action<IWebDriver> testAction)
        {
            using (var driver = new ChromeDriver())
            {
                testAction(driver);
            }
        }
    }
}
