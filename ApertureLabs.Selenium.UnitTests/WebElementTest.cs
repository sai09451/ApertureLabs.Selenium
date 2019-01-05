using System;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects.HomePage;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class WebElementTest
    {
        #region Fields

        private static IPageObjectFactory pageObjectFactory;
        private static WebDriverFactory webDriverFactory;

        #endregion

        #region Startup/Teardown

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            pageObjectFactory = new PageObjectFactory();
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void Teardown()
        {
            webDriverFactory.Dispose();
        }

        #endregion

        #region Tests

        [ServerRequired]
        [TestMethod]
        public void GetPropertyTest()
        {
            var driver = webDriverFactory.CreateDriver(MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            using (driver)
            {
                var homePage = pageObjectFactory.PreparePage(new HomePage(driver,
                    Startup.ServerUrl,
                    pageObjectFactory));

                var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
                var text = el.Text;
                var prop = el.GetProperty("host");

                Assert.IsFalse(String.IsNullOrEmpty(prop));
            }
        }

        [ServerRequired]
        [TestMethod]
        public void GetPropertyPolyfillTest()
        {
            var driver = webDriverFactory.CreateDriver(MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            using (driver)
            {
                var homePage = pageObjectFactory.PreparePage(new HomePage(driver,
                    Startup.ServerUrl,
                    pageObjectFactory));

                var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
                var text = el.Text;
                var prop = el.GetElementProperty("host");

                Assert.IsFalse(String.IsNullOrEmpty(prop));
            }
        }

        [ServerRequired]
        [TestMethod]
        public void GetCssValueTest()
        {
            var driver = webDriverFactory.CreateDriver(MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            using (driver)
            {
                var homePage = pageObjectFactory.PreparePage(new HomePage(driver,
                    Startup.ServerUrl,
                    pageObjectFactory));

                var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
                var color = el.GetCssValue("color");

                Assert.IsFalse(String.IsNullOrEmpty(color));
            }
        }

        #endregion
    }
}
