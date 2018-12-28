using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KendoPagerComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;
        private static IWebDriver WebDriver;

        #endregion

        #region Setup/Teardown

        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        #endregion

        #region Tests

        [DataTestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        [DataRow(MajorWebDriver.Edge)]
        [DataRow(MajorWebDriver.Firefox)]
        [ServerRequired]
        public void KendoPagerComponentTest(MajorWebDriver driverType)
        {
            var driver = WebDriverFactory.CreateDriver(
                driverType,
                WindowSize.DefaultDesktop);
        }

        #endregion
    }
}
