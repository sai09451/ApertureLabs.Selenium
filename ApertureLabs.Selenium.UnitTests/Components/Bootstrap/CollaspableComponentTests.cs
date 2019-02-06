using ApertureLabs.Selenium.Components.Boostrap.Collapsable;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.Bootstrap
{
    [TestClass]
    public class CollaspableComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private CollapsableComponent<WidgetPage> collaspableComponent;
        private IWebDriver driver;
        private IPageObjectFactory pageObjectFactory;
        private WidgetPage widgetPage;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void TearDown()
        {
            webDriverFactory?.Dispose();
        }

        [TestInitialize]
        public void TestSetup()
        {
            driver = webDriverFactory.CreateDriver(
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
                    "Bootstrap",
                    "4.1",
                    "Collapsable");

            collaspableComponent = pageObjectFactory.PrepareComponent(
                new CollapsableComponent<WidgetPage>(
                    new CollapsableOptions(
                        By.CssSelector("#multiCollapseExample1"),
                        new[] { By.CssSelector("#toggle-first") }),
                    driver,
                    widgetPage));
        }

        #endregion

        #region Tests

        [Description("Checks that no exceptions are thrown in the ctor.")]
        [ServerRequired]
        [TestMethod]
        public void CollaspableComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetAllOpenElementsTest()
        {
            var firstOpenEls = collaspableComponent.GetAllOpenElements();
            var secondOpenEls = collaspableComponent.GetAllOpenElements(true);

            Assert.IsTrue(firstOpenEls.Count == 1);
            Assert.IsTrue(secondOpenEls.Count == 0);
        }

        [ServerRequired]
        [TestMethod]
        public void GetAllCloseElementsTest()
        {
            var firstCloseEls = collaspableComponent.GetAllCloseElements();
            var secondCloseEls = collaspableComponent.GetAllCloseElements(true);

            Assert.IsTrue(firstCloseEls.Count == 1);
            Assert.IsTrue(secondCloseEls.Count == 0);
        }

        [ServerRequired]
        [TestMethod]
        public void OpenTest()
        {
            var firstIsOpen = collaspableComponent.IsExpanded();
            collaspableComponent.Open();
            var secondIsOpen = collaspableComponent.IsExpanded();

            Assert.IsFalse(firstIsOpen);
            Assert.IsTrue(secondIsOpen);
        }

        #endregion
    }
}
