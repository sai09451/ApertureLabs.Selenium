using System;
using System.Text.RegularExpressions;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium
{
    [TestClass]
    public class SeleniumNodeTests
    {
        #region Fields

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        #endregion

        #region Tests

        [TestMethod]
        public void GetCommandLineArgumentsTest()
        {
            var node = new SeleniumNode();
            var privateHelper = new PrivateObject(node);
            var result = (string)privateHelper.Invoke("GetCommandLineArguments");

            TestContext.WriteLine($"Result: {result}");

            // Verify the results aren't empty/null.
            Assert.IsFalse(String.IsNullOrEmpty(result));

            // Must have the hub flag defined.
            StringAssert.Matches(result, new Regex(@"\s-hub\shttp:[^\s]+"));

            // Shouldn't contain more than one consecutive space.
            StringAssert.DoesNotMatch(result, new Regex(@"\s{2,}"));
        }

        #endregion
    }
}
