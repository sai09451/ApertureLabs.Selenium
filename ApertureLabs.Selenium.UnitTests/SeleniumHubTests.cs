using System;
using System.Linq;
using System.Text.RegularExpressions;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace ApertureLabs.Selenium
{
    [TestClass]
    public class SeleniumHubTests
    {
        #region Fields

        public TestContext TestContext { get; set; }

        private SeleniumHub seleniumHub;

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public void TestInitialize()
        {
            seleniumHub = new SeleniumHub();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            seleniumHub.Dispose();
        }

        #endregion

        #region Methods

        [Ignore]
        [Description("Verifies no exceptions are thrown in the ctor.")]
        [TestMethod]
        public void SeleniumHubTest()
        {
            Assert.IsNotNull(seleniumHub);
        }

        [TestMethod]
        public void StartTest1()
        {
            seleniumHub.StartProcess();
        }

        [TestMethod]
        public void StartTest2()
        {
            var opts = seleniumHub.Options;
            opts.Log = null;
            seleniumHub.StartProcess();
        }

        [TestMethod]
        public void StopTest()
        {
            seleniumHub.StartProcess();
            seleniumHub.StopProcess();
        }

        [TestMethod]
        public void RegisterNodeTest()
        {
            seleniumHub.StartProcess();
            seleniumHub.RegisterNode(new SeleniumNodeOptions());

            // Try and activate the ChromeDriver.
            var dm = new DriverManager();
            dm.SetUpDriver(new ChromeConfig());

            var remoteWebDriver = new RemoteWebDriver(
                new Uri("http://127.0.0.1:4444/wd/hub"),
                new ChromeOptions());

            remoteWebDriver.Dispose();

            Assert.IsNotNull(remoteWebDriver);
        }

        [DataTestMethod]
        [DataRow(1, DisplayName = "First run")]
        [DataRow(2, DisplayName = "Second run")]
        public void UnregisterNodeTest(int run)
        {
            var opts = seleniumHub.Options;
            opts.Log = null;
            RegisterNodeTest();
            var node = seleniumHub.GetRegisteredNodes().First();
            seleniumHub.UnregisterNode(node);
        }

        [TestMethod]
        public void GetCommandLineArgumentsTest()
        {
            var privateObject = new PrivateObject(seleniumHub);
            var result = (string)privateObject.Invoke("GetCommandLineArguments");

            TestContext.WriteLine($"Result: {result}");

            // Verify the results aren't empty/null.
            Assert.IsFalse(String.IsNullOrEmpty(result));

            // Shouldn't contain more than one consecutive space.
            StringAssert.DoesNotMatch(result, new Regex(@"\s{2,}"));
        }

        #endregion
    }
}
