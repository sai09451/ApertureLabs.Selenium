using System;
using System.Linq;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KPager;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects.HomePage;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KendoPagerComponentTests
    {
        #region Fields

        private static KPagerComponent PagerComponent;
        private static IPageObjectFactory PageObjectFactory;
        private static WebDriverFactory WebDriverFactory;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            PageObjectFactory = new PageObjectFactory();
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void ClassTeardown()
        {
            WebDriverFactory.Dispose();
        }

        #endregion

        #region Tests

        [DataTestMethod]
        [DataRow(MajorWebDriver.Chrome)]
        //[DataRow(MajorWebDriver.Edge)]
        //[DataRow(MajorWebDriver.Firefox)]
        [ServerRequired]
        public void KendoPagerComponentTest(MajorWebDriver driverType)
        {
            var driver = WebDriverFactory.CreateDriver(
                driverType,
                WindowSize.DefaultDesktop);

            using (driver)
            {
                var homePage = PageObjectFactory.PreparePage(new HomePage(driver,
                    Startup.ServerUrl,
                    PageObjectFactory));

                var widgetPage = homePage.GoToWidget("kendo",
                    "2014.1.318",
                    "KPager");

                var pagerComponent = PageObjectFactory.PrepareComponent(
                    new KPagerComponent(driver,
                        By.CssSelector("#pager"),
                        DataSourceOptions.DefaultKendoOptions(),
                        PageObjectFactory));

                pagerComponent.Refresh();
                var availbleSizes = pagerComponent.GetAvailableItemsPerPage();
                var activePageSize = pagerComponent.GetItemsPerPage();
                var totalItems = pagerComponent.GetTotalItems();
                var initialActivePage = pagerComponent.GetActivePage();
                pagerComponent.NextPage();
                var secondActivePage = pagerComponent.GetActivePage();
                pagerComponent.PrevPage();
                pagerComponent.LastPage();
                pagerComponent.FirstPage();

                CollectionAssert.AreEqual(new[] { 2, 4 }, availbleSizes.ToArray());
                Assert.AreEqual(totalItems, 4);
                Assert.IsTrue(pagerComponent.HasNextPage);
                Assert.IsFalse(pagerComponent.HasPreviousPage);
                Assert.AreEqual(initialActivePage, 1);
                Assert.AreEqual(secondActivePage, 2);
                Assert.AreEqual(2, activePageSize);
            }
        }

        #endregion
    }
}
