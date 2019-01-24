using System;
using System.Drawing;
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
    public class TinyMCEComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private TinyMCEComponent tinyMCE;

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
        public void TinyMCETest()
        {
            Assert.IsTrue(tinyMCE.IntegrationMode == IntegrationMode.Classic);
        }

        [ServerRequired]
        [TestMethod]
        public void WriteTest()
        {
            var firstText = tinyMCE.GetContent();
            tinyMCE.Write("Testing 1 2 3" + Keys.Enter + "43." + Keys.Enter);
            var secondText = tinyMCE.GetContent();

            Assert.IsTrue(String.Equals(firstText,
                String.Empty,
                StringComparison.Ordinal));

            Assert.IsFalse(String.Equals(firstText,
                secondText,
                StringComparison.Ordinal));

            Assert.IsFalse(String.IsNullOrEmpty(secondText));
        }

        [ServerRequired]
        [TestMethod]
        public void WriteLineTest()
        {
            var firstText = tinyMCE.GetContent();
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.WriteLine("Testing 4 5 6");
            var secondText = tinyMCE.GetContent();

            Assert.IsTrue(String.Equals(firstText,
                String.Empty,
                StringComparison.Ordinal));

            Assert.IsFalse(String.Equals(firstText,
                secondText,
                StringComparison.Ordinal));

            Assert.IsFalse(String.IsNullOrEmpty(secondText));
        }

        [Description("Same as WriteLineTest.")]
        [ServerRequired]
        [TestMethod]
        public void GetContentTest()
        {
            WriteLineTest();
        }

        [ServerRequired]
        [TestMethod]
        public void ClearAllContent()
        {
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.WriteLine("Testing 4 5 6");
            var beforeClear = tinyMCE.GetContent();
            tinyMCE.ClearAllContent();
            var afterClear = tinyMCE.GetContent();

            Assert.AreNotEqual(beforeClear, afterClear);
            Assert.IsTrue(String.IsNullOrEmpty(afterClear));
        }

        [ServerRequired]
        [TestMethod]
        public void GetCursorPositionTest()
        {
            var firstPos = tinyMCE.GetCursorPosition();
            tinyMCE.Write("Testing" + Keys.Enter + "1 2 3");
            var secondPos = tinyMCE.GetCursorPosition();

            Assert.AreEqual(firstPos, new Point(0, 0));
            Assert.AreEqual(secondPos, new Point(5, 1));
        }

        [ServerRequired]
        [TestMethod]
        public void SetCursorPositionTest()
        {
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.SetCursorPosition(new Point(4, 0));
            var position = tinyMCE.GetCursorPosition();

            Assert.AreEqual(position, new Point(4, 0));
        }

        [ServerRequired]
        [TestMethod]
        public void HighlightRangeTest()
        {
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.WriteLine("Testing 1 2 3");
            tinyMCE.WriteLine("Testing 1 2 3");

            tinyMCE.HightlightRange(new Point(0, 0), new Point(5, 1));

            Console.WriteLine();
        }

        #endregion
    }
}
