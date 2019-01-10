using System.Linq;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KPager;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KPagerComponentTests
    {
        #region Fields

        private static KPagerComponent PagerComponent;
        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory PageObjectFactory;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
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

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(driver);
            serviceCollection.AddSingleton(new PageOptions
            {
                Url = Startup.ServerUrl
            });

            PageObjectFactory = new PageObjectFactory(serviceCollection, true);

            using (driver)
            {
                var homePage = PageObjectFactory.PreparePage<HomePage>();

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
