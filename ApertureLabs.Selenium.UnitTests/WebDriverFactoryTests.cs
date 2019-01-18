using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class WebDriverFactoryTests
    {
        private static WebDriverFactory WebDriverFactory;

        public TestContext TestContext { get; set; }

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

        [DataTestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        [DataRow(MajorWebDriver.Firefox)]
        [DataRow(MajorWebDriver.Edge)]
        [DataRow(MajorWebDriver.InternetExplorer)]
        public void GetWebDriver(MajorWebDriver majorWebDriver)
        {
            var driver = WebDriverFactory.CreateDriver(
                majorWebDriver,
                WindowSize.DefaultDesktop);

            using (driver)
            {
                var defaultUrl = driver.Url;
            }
        }
    }
}
