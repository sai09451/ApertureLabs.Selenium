using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium
{
    [TestClass]
    public class WebDriverFactoryTests
    {
        private WebDriverFactory WebDriverFactory;
        public TestContext TestContext { get; set; }

        [TestCleanup]
        public void TearDown()
        {
            WebDriverFactory?.Dispose();
        }

        [TestMethod]
        public void WebDriverFactoryTest()
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [DataTestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        //[DataRow(MajorWebDriver.Firefox)]
        //[DataRow(MajorWebDriver.Edge)]
        //[DataRow(MajorWebDriver.InternetExplorer)]
        public void GetWebDriver(MajorWebDriver majorWebDriver)
        {
            WebDriverFactory = new WebDriverFactory();

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
