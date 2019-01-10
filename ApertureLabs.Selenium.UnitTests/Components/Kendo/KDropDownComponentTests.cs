using System;
using System.Reflection;
using ApertureLabs.Selenium.Components.Kendo;
using ApertureLabs.Selenium.Components.Kendo.KDropDown;
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
    public class KDropDownComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private KDropDownComponent kDropDownComponent;
        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private WidgetPage widgetPage;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            webDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestStartup()
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
                "KDropDown");

            kDropDownComponent = new KDropDownComponent(
                driver,
                By.CssSelector("#dropdown"),
                DataSourceOptions.DefaultKendoOptions(),
                KDropDownAnimationOptions.Default());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            widgetPage = null;
            driver.Dispose();
        }

        #endregion

        #region Tests

        [ServerRequired]
        [TestMethod]
        public void LoadTest()
        {
            // Make sure no errors are thrown here.
            kDropDownComponent.Load();
        }

        [ServerRequired]
        [TestMethod]
        public void IsExpandedTest()
        {
            kDropDownComponent = pageObjectFactory
                .PrepareComponent(kDropDownComponent);

            var initialExpansion = kDropDownComponent.IsExpanded();

            var type = kDropDownComponent.GetType();
            type.GetMethod("Expand", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(kDropDownComponent, null);

            var secondExpansion = kDropDownComponent.IsExpanded();
            type.GetMethod("Close", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(kDropDownComponent, null);

            var thirdExpansion = kDropDownComponent.IsExpanded();

            Assert.IsFalse(initialExpansion);
            Assert.IsTrue(secondExpansion);
            Assert.IsFalse(thirdExpansion);
        }

        [ServerRequired]
        [TestMethod]
        public void GetSelectedItemTest()
        {
            kDropDownComponent = pageObjectFactory
                .PrepareComponent(kDropDownComponent);

            var initialSelectedItem = kDropDownComponent.GetSelectedItem();
            kDropDownComponent.SetSelectedItem("5");
            var secondSelectedItem = kDropDownComponent.GetSelectedItem();

            Assert.AreEqual(initialSelectedItem, "2");
            Assert.AreEqual(secondSelectedItem, "5");
        }

        [Description("Same as the GetSelectedItemTest.")]
        [ServerRequired]
        [TestMethod]
        public void SetSelectedItemsTest()
        {
            GetSelectedItemTest();
        }

        [ServerRequired]
        [TestMethod]
        public void WaitForAnimationStartTest()
        {
            throw new NotImplementedException();
        }

        [ServerRequired]
        [TestMethod]
        public void WaitForAnimationEndTest()
        {
            throw new NotImplementedException();
        }

        [ServerRequired]
        [TestMethod]
        public void IsCurrentlyAnimatingTest()
        {
            throw new NotImplementedException();
        }

        [ServerRequired]
        [TestMethod]
        public void ExpandTest()
        {
            throw new NotImplementedException();
        }

        [ServerRequired]
        [TestMethod]
        public void CloseTest()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
