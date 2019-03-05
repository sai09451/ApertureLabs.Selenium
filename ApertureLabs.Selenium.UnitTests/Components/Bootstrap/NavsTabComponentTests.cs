using System.Linq;
using ApertureLabs.Selenium.Components.Boostrap.Navs;
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
    public class NavsTabComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private WidgetPage widgetPage;
        private NavsTabComponent<WidgetPage> navsTabComponent;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();
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
                "Bootstrap",
                "4.1",
                "NavsTab");

            navsTabComponent = pageObjectFactory.PrepareComponent(
                new NavsTabComponent<WidgetPage>(
                    By.CssSelector(".container.body-content"),
                    driver,
                    new NavsTabComponentConfiguration(),
                    widgetPage));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            webDriverFactory.Dispose();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Dispose();
        }

        #endregion

        #region Tests

        [Description("Checks that no exceptions are thrown.")]
        [ServerRequired]
        [TestMethod]
        public void NavsTabComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetTabNamesTest()
        {
            var expected = new[] { "Home", "Profile", "Contact" };
            var tabNames = navsTabComponent.GetTabNames().ToArray();

            CollectionAssert.AreEqual(
                expected,
                tabNames);
        }

        [ServerRequired]
        [TestMethod]
        public void GetActiveTabNameTest()
        {
            var firstActiveTabName = navsTabComponent.GetActiveTabName();
            navsTabComponent.SetActiveTab("Profile");
            var secondActiveTabName = navsTabComponent.GetActiveTabName();

            Assert.AreEqual(firstActiveTabName, "Home");
            Assert.AreEqual(secondActiveTabName, "Profile");
        }

        [Description("Identical to GetActiveTabNameTest().")]
        [ServerRequired]
        [TestMethod]
        public void SetActiveTabNameTest()
        {
            GetActiveTabNameTest();
        }

        [ServerRequired]
        [TestMethod]
        public void GetActiveTabBodyTest()
        {
            var firstActiveBody = navsTabComponent.GetActiveTabBody();
            var copyOfFirstActiveBody = navsTabComponent.GetActiveTabBody();
            navsTabComponent.SetActiveTab("Profile");
            var secondActiveBody = navsTabComponent.GetActiveTabBody();

            Assert.AreNotEqual(firstActiveBody, secondActiveBody);
        }

        #endregion
    }
}
