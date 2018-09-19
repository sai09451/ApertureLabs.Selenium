using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class WebDriverFactoryTests
    {
        private static WebDriverFactory WebDriverFactory;

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            WebDriverFactory.Dispose();
        }

        [TestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        [DataRow(MajorWebDriver.Firefox)]
        [DataRow(MajorWebDriver.Edge)]
        public void GetWebDriver(MajorWebDriver majorWebDriver)
        {
            IWebDriver chrome = WebDriverFactory.CreateDriver(majorWebDriver,
                new PageObjects.WindowSize(1000, 1001));

            using (chrome)
            {
                var defaultUrl = chrome.Url;
            }
        }
    }
}
