using ApertureLabs.Selenium.Components.TinyMCE;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.TinyMCE
{
    [TestClass]
    public class MenuComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private TinyMCEComponent tinyMCE;
        private MenuComponent menu;

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
        public void TestStartup()
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

            pageObjectFactory.PreparePage<HomePage>()
                .GoToWidget(
                    "TinyMCE",
                    "4.0",
                    "TinyMCE");

            tinyMCE = pageObjectFactory.PrepareComponent(
                new TinyMCEComponent(
                    By.CssSelector("textarea"),
                    pageObjectFactory,
                    driver,
                    TinyMCEOptions.Default()));

            menu = tinyMCE.Menu;
        }

        public void TestCleanup()
        {
            driver?.Dispose();
        }

        #endregion

        #region Tests

        [Description("Verifies no errors are thrown when the component is loading.")]
        [ServerRequired]
        [TestMethod]
        public void MenuTest()
        {
            Assert.IsNotNull(menu);
        }

        [ServerRequired]
        [TestMethod]
        public void GetItemByText()
        {
            var cut = menu.GetItemByText("Edit")
                .AsDropDown()
                .SelectOption("Cut");

            cut.AsElement().Click();

            Assert.IsNotNull(cut);
        }

        #endregion
    }
}
