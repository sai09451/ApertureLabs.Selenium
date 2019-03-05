using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace ApertureLabs.Selenium
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
            var mockElement = new MockTextWebElement(text: elementText,
                scriptResult: elementText);
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
            var mockElement = new MockTextWebElement(text: elementText,
                scriptResult: elementText);
            var ints = mockElement.TextHelper()
                .ExtractIntegers()
                .ToArray();

            CollectionAssert.AreEqual(ints, expectedInts);
        }

        [DataTestMethod]
        [DataRow("Todays year is 1994.", "yyyy")]
        [DataRow("Todays year is Dec 3 of 1994.", @"MMM d o\f yyyy")]
        [DataRow("Todays year is 2/24/1994.", @"M/dd/yyyy")]
        [DataRow("Todays year is Dec 3 of 1994.", @"MMM d 'of' yyyy")]
        public void ExtractDateTimeTest(string elementText, string dateTimeFormat)
        {
            var el = new MockTextWebElement(text: elementText,
                scriptResult: elementText);

            var dateTime = el.TextHelper().ExtractDateTime(dateTimeFormat);

            Assert.AreEqual(dateTime.Year, 1994);
        }

        #endregion
    }
}
