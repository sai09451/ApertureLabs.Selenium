using ApertureLabs.Selenium.Components.Kendo.KDatePicker;
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

namespace ApertureLabs.Selenium.UnitTests.Components.Kendo
{
    [TestClass]
    public class KDatePickerComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private KDatePickerComponent<WidgetPage> kDatePicker;
        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private WidgetPage widgetPage;

        #endregion

        #region Setup/Cleanup

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
                "KDatePicker");

            kDatePicker = pageObjectFactory.PrepareComponent(
                new KDatePickerComponent<WidgetPage>(
                    new KDatePickerConfiguration(),
                    By.CssSelector("#datepicker"),
                    driver,
                    widgetPage));
        }

        #endregion

        #region Tests

        [Ignore]
        [Description("Verifies no errors are thrown in ctor.")]
        [ServerRequired]
        [TestMethod]
        public void KDatePickerComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void ExpandTest()
        {
            // Should be able to call expand multiple times
            kDatePicker.Expand();
            kDatePicker.Expand();
            kDatePicker.Expand();
            kDatePicker.Expand();
        }

        [ServerRequired]
        [TestMethod]
        public void CollapseTest()
        {
            // Should be able to call collapse multiple times without error.
            kDatePicker.Collapse();
            kDatePicker.Collapse();
            kDatePicker.Collapse();
            kDatePicker.Collapse();
        }

        [ServerRequired]
        [TestMethod]
        public void GetValueTest()
        {
            var now = new DateTime(2019, 3, 4);

            var firstValue = kDatePicker.GetValue();
            kDatePicker.SetValue(now);
            var secondValue = kDatePicker.GetValue();
            kDatePicker.SetValue(null);
            var thirdValue = kDatePicker.GetValue();

            Assert.IsNull(firstValue);
            Assert.AreEqual(secondValue, now);
            Assert.IsNull(thirdValue);
        }

        [Description("Identical to GetValueTest.")]
        [Ignore]
        [ServerRequired]
        [TestMethod]
        public void SetValueTest()
        {
            GetValueTest();
        }

        #endregion
    }
}
