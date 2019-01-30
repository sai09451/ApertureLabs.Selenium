using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium
{
    [TestClass]
    public class WebDriverFactoryTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            WebDriverFactory?.Dispose();
        }

        #endregion

        [Description("Verifies no execptions are thrown.")]
        [TestMethod]
        public void WebDriverFactoryTest()
        { }

        [DataTestMethod]
        //[DataRow(MajorWebDriver.Chrome)]
        [DataRow(MajorWebDriver.Firefox)]
        //[DataRow(MajorWebDriver.Edge)]
        //[DataRow(MajorWebDriver.InternetExplorer)]
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
