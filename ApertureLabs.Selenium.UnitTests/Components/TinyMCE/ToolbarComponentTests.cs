using ApertureLabs.Selenium.Components.TinyMCE;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace ApertureLabs.Selenium.UnitTests.Components.TinyMCE
{
    [TestClass]
    public class ToolbarComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;
        private static IPageObjectFactory pageObjectFactory;
        private static IWebDriver driver;
        private TinyMCEComponent tinyMCE;

        private ToolbarComponent toolbar;

        #endregion

        #region Setup/Cleanup

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();

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
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            WebDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestStartup()
        {
            tinyMCE = pageObjectFactory.PrepareComponent(
                new TinyMCEComponent(
                    By.CssSelector("textarea"),
                    pageObjectFactory,
                    driver,
                    new TinyMCEOptions()));

            toolbar = tinyMCE.Toolbar;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver.Navigate().Refresh();
        }

        #endregion

        #region Tests

        [Description("Verifies ctor throws no errors.")]
        [Ignore]
        [ServerRequired]
        [TestMethod]
        public void ToolbarComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetItemByClassTest()
        {
            var className = "mce-i-alignleft";
            tinyMCE.WriteLine("Testing 'GetItemByClass(...)'.");

            var bttnGroup = toolbar
                .GetItemByClass(className)
                .ConvertTo<ButtonGroupMenuItem>();

            var btn = bttnGroup.GetItemByClass(className);
            btn.WrappedElement.Click();

            Assert.IsTrue(btn.WrappedElement.Classes().Contains("mce-active"));
            Assert.IsTrue(btn.HasIcon);
        }

        [ServerRequired]
        [TestMethod]
        public void GetItemByTextTest()
        {
            tinyMCE.WriteLine("Testing 'GetItemByText(...)'.");

            var formatBttn = toolbar
                .GetItemByText("Formats")
                .ConvertTo<DropDownMenuItem>();

            formatBttn
                .SelectOption<DropDownMenuItem>("Headings")
                .SelectOption("Heading 1")
                .AsElement()
                .Click();


            Assert.IsNotNull(formatBttn);
        }

        [ServerRequired]
        [TestMethod]
        public void GetMenuItemAtTest()
        {
            // Use Write here so that clicking
            tinyMCE.Write("Testing 'GetMenuItemAtTest()'.");

            // Should click the undo button.
            toolbar
                .GetMenuItemAt(0)
                .ConvertTo<ButtonGroupMenuItem>()
                .GetMenuItemAt(0)
                .AsElement()
                .Click();

            var content = tinyMCE.GetContent();

            Assert.IsTrue(String.IsNullOrEmpty(content));
        }

        [ServerRequired]
        [TestMethod]
        public void GetMenuItemsTest()
        {
            var menuItems = toolbar.GetMenuItems();

            Assert.IsTrue(menuItems.Any());
        }

        [DataTestMethod]
        [DataRow("mce-i-alignleft", true)]
        [DataRow("fake-class", false)]
        [ServerRequired]
        public void HasItemWithClass(string className, bool expected)
        {
            var hasItem = toolbar.HasItemWithClass(className);

            Assert.AreEqual(expected, hasItem);
        }

        [DataTestMethod]
        [DataRow("Formats", true)]
        [DataRow("Fake", false)]
        [ServerRequired]
        public void HasItemWithText(string text, bool expected)
        {
            var hasItem = toolbar.HasItemWithText(text);

            Assert.AreEqual(expected, hasItem);
        }

        #endregion
    }
}
