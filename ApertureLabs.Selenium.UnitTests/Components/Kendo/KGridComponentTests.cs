using System;
using System.Linq;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KGrid;
using ApertureLabs.Selenium.Extensions;
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
            PrepareComponent();
        }

        [ServerRequired]
        [TestMethod]
        public void GetColumnHeadersTest()
        {
            var kGridComponent = PrepareComponent();
            var columnHeaders = kGridComponent.GetColumnHeaders()
                .ToArray();

            CollectionAssert.AreEqual(columnHeaders.ToArray(), new[] { "name", "age" });
        }

        [ServerRequired]
        [TestMethod]
        public void GetCellTest()
        {
            var kGridComponent = PrepareComponent();
            var cell_0_0 = kGridComponent.GetCell(0, 0);
            var cell_0_1 = kGridComponent.GetCell(0, 1);

            var name = cell_0_0.TextHelper().InnerText;
            var age = cell_0_1.TextHelper().ExtractInteger();

            Assert.IsTrue(String.Equals(name,
                "Jane Doe",
                StringComparison.Ordinal));
            Assert.AreEqual(age, 30);
        }

        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [ServerRequired]
        [TestMethod]
        public void GetCellNegativeTest()
        {
            var kGridComponent = PrepareComponent();
            kGridComponent.GetCell(0, 3);
        }

        [ServerRequired]
        [TestMethod]
        public void KPagerComponentTest()
        {
            var kGridComponent = PrepareComponent();

            var firstname = kGridComponent.GetCell(0, 0)
                .TextHelper()
                .InnerText;

            kGridComponent.Pager.SetItemsPerPage(5);
            kGridComponent.Pager.NextPage();

            var secondName = kGridComponent.GetCell(0, 0)
                .TextHelper()
                .InnerText;

            Assert.IsFalse(String.Equals(firstname,
                secondName,
                StringComparison.Ordinal));
        }

        private KGridComponent PrepareComponent()
        {
            return pageObjectFactory.PrepareComponent(
                new KGridComponent(
                    driver,
                    By.CssSelector("#grid"),
                    DataSourceOptions.DefaultKendoOptions(),
                    pageObjectFactory));
        }

        #endregion
    }
}
