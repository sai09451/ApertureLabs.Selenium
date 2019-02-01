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

namespace ApertureLabs.Selenium.UnitTests.Extensions
{
    [TestClass]
    public class IWebDriverExtensionsTest
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private HomePage homePage;
        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            webDriverFactory?.Dispose();
        }

        [TestInitialize]
        public void TestSetup()
        {
            driver = webDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(driver);
            serviceCollection.AddSingleton(new PageOptions
            {
                Url = Startup.ServerUrl
            });

            pageObjectFactory = new PageObjectFactory(serviceCollection);
            homePage = pageObjectFactory.PreparePage<HomePage>();
        }

        #endregion

        #region Tests

        [Description("User must enter anything in the textarea for this " +
            "test to pass.")]
        [ServerRequired]
        [TestMethod]
        public void WaitForUserSignalTest()
        {
            var response = driver.WaitForUserSignal(TimeSpan.FromSeconds(30));

            Assert.IsFalse(String.IsNullOrEmpty(response));
        }

        [ExpectedException(typeof(WebDriverException), AllowDerivedTypes = true)]
        [TestMethod]
        public void WaitForUserSignalNegativeTest()
        {
            // Should throw an exception.
            driver.WaitForUserSignal(TimeSpan.FromMilliseconds(20));
        }

        #endregion
    }
}
