using System;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class WebElementTest
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private static IWebDriver driver;

        #endregion

        #region Startup/Teardown

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void Teardown()
        {
            WebDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            driver = WebDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(driver);
            serviceCollection.AddSingleton(new PageOptions
            {
                Url = Startup.ServerUrl
            });

            pageObjectFactory = new PageObjectFactory(serviceCollection, true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Dispose();
        }

        #endregion

        #region Tests

        [ServerRequired]
        [TestMethod]
        public void GetPropertyTest()
        {
            var homePage = pageObjectFactory.PreparePage<HomePage>();

            var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
            var text = el.Text;
            var prop = el.GetProperty("host");

            Assert.IsFalse(String.IsNullOrEmpty(prop));
        }

        [ServerRequired]
        [TestMethod]
        public void GetPropertyPolyfillTest()
        {
            var homePage = pageObjectFactory.PreparePage<HomePage>();

            var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
            var text = el.Text;
            var prop = el.GetElementProperty("host");

            Assert.IsFalse(String.IsNullOrEmpty(prop));
        }

        [ServerRequired]
        [TestMethod]
        public void GetCssValueTest()
        {
            var homePage = pageObjectFactory.PreparePage<HomePage>();

            var el = driver.FindElement(By.CssSelector("#navbarSupportedContent > ul > li > a"));
            var color = el.GetCssValue("color");

            Assert.IsFalse(String.IsNullOrEmpty(color));
        }

        #endregion
    }
}
