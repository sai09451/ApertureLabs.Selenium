using System.Drawing;
using System.Linq;
using ApertureLabs.Selenium.Components.TinyMCE;
using ApertureLabs.Selenium.PageObjects;
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
            var beforeCut = "Testing 1 2 3" + Keys.Enter;

            tinyMCE.Write(beforeCut);
            tinyMCE.HightlightRange(new Point(7, 0), new Point(12, 0));
            menu.GetItemByText("Edit")
                .ConvertTo<DropDownMenuItem>()
                .SelectOption("Cut")
                .AsElement()
                .Click();

            // Highlight all text.
            tinyMCE.HighlightAllText();
            var afterCut = tinyMCE.GetHighlightedText();

            Assert.AreNotEqual(afterCut, beforeCut);
        }

        [Description("Don't need to test this as all menu items shouldn't " +
            "have icons.")]
        [Ignore]
        [ServerRequired]
        [TestMethod]
        public void GetItemByClassTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void HasItemWithTextTest()
        {
            var hasEdit = tinyMCE.Menu.HasItemWithText("Edit");

            Assert.IsTrue(hasEdit);
        }

        [Description("Don't need to test this as all menu items shouldn't " +
            "have icons.")]
        [Ignore]
        [ServerRequired]
        [TestMethod]
        public void HasItemsWithClassTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetMenuItemsTest()
        {
            var menuItems = tinyMCE.Menu.GetMenuItems().ToArray();

            CollectionAssert.AllItemsAreInstancesOfType(
                menuItems,
                typeof(MenuItem));

            CollectionAssert.AllItemsAreNotNull(menuItems);

            CollectionAssert.AllItemsAreUnique(menuItems);

            Assert.IsTrue(menuItems.Any());
        }

        #endregion
    }
}
