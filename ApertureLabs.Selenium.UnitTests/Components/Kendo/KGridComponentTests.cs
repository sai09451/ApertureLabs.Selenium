using System.Linq;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KGrid;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects.HomePage;
using MockServer.PageObjects.WidgetPage;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KGridComponentTests
    {
        #region Fields

        private static IPageObjectFactory pageObjectFactory;
        private static IWebDriver driver;
        private static WebDriverFactory webDriverFactory;
        private static WidgetPage widgetPage;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            pageObjectFactory = new PageObjectFactory();
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            webDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            driver = webDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var homePage = pageObjectFactory.PreparePage(
                new HomePage(driver, Startup.ServerUrl, pageObjectFactory));

            widgetPage = homePage.GoToWidget("kendo", "2014.1.318", "KGrid");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Dispose();
        }

        #endregion

        #region Tests

        [Description("Will always pass. Don't need to test the ctor.")]
        [TestMethod()]
        public void KGridComponentTest()
        { }

        [Description("Checks that no errors are thrown when loading the page.")]
        [ServerRequired]
        [TestMethod()]
        public void LoadTest()
        {
            var kGridComponent = new KGridComponent(driver,
                By.CssSelector(""),
                DataSourceOptions.DefaultKendoOptions(),
                pageObjectFactory);

            kGridComponent.Load();
        }

        [ServerRequired]
        [TestMethod]
        public void GetColumnHeadersTest()
        {
            var kGridComponent = pageObjectFactory.PrepareComponent(
                new KGridComponent(
                    driver,
                    By.CssSelector(""),
                    DataSourceOptions.DefaultKendoOptions(),
                    pageObjectFactory));

            var columnHeaders = kGridComponent.GetColumnHeaders()
                .ToArray();

            CollectionAssert.AreEqual(columnHeaders.ToArray(), new[] { "name", "age" });
        }

        #endregion
    }
}
