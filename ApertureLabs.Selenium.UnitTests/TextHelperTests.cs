using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Linq;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class TextHelperTests
    {
        #region Fields

        private static IWebElement Element;

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            Element = new MockTextWebElement();
        }

        #endregion

        #region Methods

        [DataTestMethod]
        [DataRow("1 - 15 of 2383 items", new[] { 1d, 15d, 2383d })]
        [DataRow("1.23 - 15.53 of 2383 items", new[] { 1.23d, 15.53d, 2383d })]
        public void ExtractDoublesTest(string elementText,
            double[] expectedDoubles)
        {
            var mockElement = new MockTextWebElement(text: elementText);
            var doubles = mockElement.TextHelper()
                .ExtractDoubles()
                .ToArray();

            CollectionAssert.AreEqual(doubles, expectedDoubles);
        }

        [DataTestMethod]
        [DataRow("1 - 15 of 2383 items", new[] { 1, 15, 2383 })]
        [DataRow("1.23 - 15.53 of 2383 items", new[] { 1, 15, 2383 })]
        public void ExtractIntegersTest(string elementText,
            int[] expectedInts)
        {
            var mockElement = new MockTextWebElement(text: elementText);
            var ints = mockElement.TextHelper()
                .ExtractIntegers()
                .ToArray();

            CollectionAssert.AreEqual(ints, expectedInts);
        }

        #endregion
    }
}
