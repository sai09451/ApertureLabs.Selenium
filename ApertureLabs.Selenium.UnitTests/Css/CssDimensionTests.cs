using ApertureLabs.Selenium.Css;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests.Css
{
    [TestClass]
    public class CssDimensionTests
    {
        [DataTestMethod]
        [DataRow("43%", 43, CssUnit.Percent)]
        public void CssDimensionTest(string dimensionStr,
            double expectedNumber,
            CssUnit expectedUnit)
        {
            var cssDim = new CssDimension(dimensionStr);

            Assert.AreEqual(cssDim.Number, expectedNumber);
            Assert.AreEqual(cssDim.UnitOfMeasurement, expectedUnit);
        }
    }
}
