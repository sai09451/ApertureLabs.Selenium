using System;
using System.Linq;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KGrid;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KGridComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private KGridComponent<WidgetPage> kGridComponent;
        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private WidgetPage widgetPage;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
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

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(driver);
            serviceCollection.AddSingleton<HomePage>();
            serviceCollection.AddSingleton(new PageOptions
            {
                Url = Startup.ServerUrl
            });

            pageObjectFactory = new PageObjectFactory(serviceCollection);

            var homePage = pageObjectFactory.PreparePage<HomePage>();
            widgetPage = homePage.GoToWidget(
                "kendo",
                "2014.1.318",
                "KGrid");

            kGridComponent = pageObjectFactory.PrepareComponent(
                new KGridComponent<WidgetPage>(
                    new BaseKendoConfiguration(),
                    By.CssSelector("#grid"),
                    pageObjectFactory,
                    driver,
                    widgetPage));
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
        { }

        [Description("Test was created in response to bug where the KGrid " +
            "ctor threw an error if the toolbar or pager were disabled. " +
            "This test verifies no errors are thrown when loading a " +
            "'minimal' grid.")]
        [ServerRequired]
        [TestMethod]
        public void LoadMinimumGridTest()
        {
            kGridComponent = new KGridComponent<WidgetPage>(
                new BaseKendoConfiguration(),
                By.CssSelector("#grid2"),
                pageObjectFactory,
                driver,
                widgetPage);
        }

        [ServerRequired]
        [TestMethod]
        public void GetColumnHeadersTest()
        {
            var columnHeaders = kGridComponent.GetColumnHeaders()
                .ToArray();

            CollectionAssert.AreEqual(columnHeaders.ToArray(), new[] { "name", "age" });
        }

        [ServerRequired]
        [TestMethod]
        public void GetCellTest()
        {
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
            kGridComponent.GetCell(0, 4);
        }

        [ServerRequired]
        [TestMethod]
        public void PagerTest()
        {
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

        [Description("Verifies no exceptions are thrown.")]
        [ServerRequired]
        [TestMethod]
        public void ToolbarTest()
        {
            var previousItemTotal = kGridComponent.Pager.GetTotalItems();

            // Click 'Add new record'.
            var createNewBtnEl = kGridComponent.Toolbar.GetItems().First();
            createNewBtnEl.Click();

            // Wait for new record to appear.
            driver.Wait(TimeSpan.FromMilliseconds(500))
                .Until(d =>
                {
                    var newTotal = kGridComponent.Pager.GetTotalItems();
                    return newTotal > previousItemTotal;
                });
        }

        [ServerRequired]
        [TestMethod]
        public void GetNumberOfColumnsTest()
        {
            var numberOfColumns = kGridComponent.GetNumberOfColumns();

            Assert.IsTrue(numberOfColumns > 0);
        }

        [ServerRequired]
        [TestMethod]
        public void GetNumberOfRowsTest()
        {
            var numberOfRows = kGridComponent.GetNumberOfRows();

            Assert.IsTrue(numberOfRows > 0);
        }

        #endregion
    }
}
