using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class WebDriverFactoryTests
    {
        private WebDriverFactory webDriverFactory;

        [ClassInitialize]
        public void Setup()
        {
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public void TearDown()
        {
            webDriverFactory.Dispose();
        }

        [TestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        [DataRow(MajorWebDriver.Firefox)]
        [DataRow(MajorWebDriver.Edge)]
        public void GetWebDriver(MajorWebDriver majorWebDriver)
        {
            IWebDriver chrome = webDriverFactory.CreateDriver(majorWebDriver);

            using (chrome)
            {
                var defaultUrl = chrome.Url;
            }
        }
    }
}
