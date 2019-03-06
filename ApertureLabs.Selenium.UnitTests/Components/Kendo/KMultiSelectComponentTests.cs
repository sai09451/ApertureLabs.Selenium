using ApertureLabs.Selenium.Components.Kendo.KMultiSelect;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KMultiSelectComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private KMultiSelectComponent<WidgetPage> kMultiSelect;
        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private WidgetPage widgetPage;

        #endregion

        #region Setup/Cleanup

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
            serviceCollection.AddSingleton(new PageOptions
            {
                Url = Startup.ServerUrl
            });

            pageObjectFactory = new PageObjectFactory(serviceCollection);

            var homePage = pageObjectFactory.PreparePage<HomePage>();
            widgetPage = homePage.GoToWidget(
                "kendo",
                "2014.1.318",
                "KMultiSelect");

            kMultiSelect = pageObjectFactory.PrepareComponent(
                new KMultiSelectComponent<WidgetPage>(
                    By.CssSelector("#my-multiselect"),
                    driver,
                    new KMultiSelectConfiguration
                    {
                        AutoClose = false,
                        AnimationOptions = new KMultiSelectAnimationOptions
                        {
                            AnimationDuration = TimeSpan.FromSeconds(1),
                            AnimationsEnabled = true
                        }
                    },
                    widgetPage));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Dispose();
        }

        #endregion

        #region Tests

        [Description("Verifies no exceptions are thrown in ctor or Load.")]
        [ServerRequired]
        [TestMethod]
        public void KMultiSelectTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetAllOptionsTest()
        {
            var allOptions = kMultiSelect
                .GetAllOptions()
                .ToList();

            Assert.IsTrue(allOptions.Any());
        }

        [ServerRequired]
        [TestMethod]
        public void GetSelectedOptionsTest()
        {
            kMultiSelect.SelectItem("Oranges");
            kMultiSelect.SelectItem("Kiwis");

            var selectedOptions = kMultiSelect
                .GetSelectedOptions()
                .ToArray();

            CollectionAssert.AreEqual(
                new[] { "Oranges", "Kiwis" },
                selectedOptions);
        }

        [Description("Identical to GetSelectedOptionsTest().")]
        [Ignore]
        [ServerRequired]
        [TestMethod]
        public void SelectItemTest()
        {
            GetSelectedOptionsTest();
        }

        [ServerRequired]
        [TestMethod]
        public void DeselectItem()
        {
            GetSelectedOptionsTest();
            kMultiSelect.DeselectItem("Kiwis");
            var selectedItems = kMultiSelect
                .GetSelectedOptions()
                .ToArray();

            CollectionAssert.AreEqual(new[] { "Oranges" }, selectedItems);
        }

        #endregion
    }
}
