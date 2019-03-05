using System;
using System.Linq;
using ApertureLabs.Selenium.Components.JQuery.Accordian;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.jQueryUI
{
    [TestClass]
    public class AccordionComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;
        private AccordionComponent<WidgetPage> accordionComponent;
        private IWebDriver driver;
        private IPageObjectFactory pageObjectFactory;
        private WidgetPage widgetPage;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            WebDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestSetup()
        {
            driver = WebDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(driver)
                .AddSingleton(new PageOptions
                {
                    Url = Startup.ServerUrl
                });

            pageObjectFactory = new PageObjectFactory(serviceCollection);

            widgetPage = pageObjectFactory.PreparePage<HomePage>()
                .GoToWidget(
                    "jQueryUI",
                    "1.12",
                    "Accordian");

            accordionComponent = pageObjectFactory.PrepareComponent(
                new AccordionComponent<WidgetPage>(
                    new AccordionComponentOptions(),
                    By.CssSelector("#accordion"),
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

        [ServerRequired]
        [TestMethod]
        public void AccordianComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetPanelNamesTest()
        {
            var panelNames = accordionComponent.GetPanelNames().ToArray();

            CollectionAssert.AreEqual(panelNames,
                new[] { "Section 1", "Section 2", "Section 3" });
        }

        [ServerRequired]
        [TestMethod]
        public void SelectPanelTest()
        {
            var panelBody = accordionComponent.SelectPanel("Section 2");

            Assert.IsNotNull(panelBody);
        }

        [ServerRequired]
        [TestMethod]
        public void ClosePanelTest()
        {
            var options = new AccordionComponentOptions();
            options.Collaspable = true;

            accordionComponent = pageObjectFactory.PrepareComponent(
                new AccordionComponent<WidgetPage>(
                    options,
                    By.CssSelector("#accordion"),
                    pageObjectFactory,
                    driver,
                    widgetPage));

            accordionComponent.ClosePanel();

            Assert.IsFalse(accordionComponent.HasOpenPanel());
        }

        [ServerRequired]
        [TestMethod]
        public void HasOpenPanelTest()
        {
            var hasOpen = accordionComponent.HasOpenPanel();

            Assert.IsTrue(hasOpen);
        }

        [ServerRequired]
        [TestMethod]
        public void GetActivePanelNameTest()
        {
            var activePanelName = accordionComponent.GetActivePanelName();

            Assert.AreEqual(activePanelName, "Section 1");
        }

        [ServerRequired]
        [TestMethod]
        public void GetActivePanelContentElementTest()
        {
            var activePanelContent = accordionComponent
                .GetActivePanelContentElement();

            Assert.IsNotNull(activePanelContent);
        }

        #endregion
    }
}
