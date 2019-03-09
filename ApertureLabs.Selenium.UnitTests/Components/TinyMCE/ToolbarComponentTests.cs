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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.UnitTests.Components.TinyMCE
{
    [TestClass]
    public class ToolbarComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private TinyMCEComponent tinyMCE;
        private ToolbarComponent toolbar;

        #endregion

        #region Setup/Cleanup

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
                    new TinyMCEOptions()));

            toolbar = tinyMCE.Toolbar;
        }

        public void TestCleanup()
        {
            driver?.Dispose();
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

        #endregion
    }
}
