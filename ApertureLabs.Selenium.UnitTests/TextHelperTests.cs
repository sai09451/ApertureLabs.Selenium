using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ApertureLabs.Selenium.UnitTests
{
    [TestClass]
    public class TextHelperTests
    {
        [TestMethod]
        public void RgbTextTest()
        {
            #region Arrange

            var r = 123;
            var g = 222;
            var b = 24;

            #endregion

            #region Act

            var color = TextHelper.FromRgbString($"rgb(123, 222, 24)");

            #endregion

            #region Assert

            Assert.AreEqual(r, color.R);
            Assert.AreEqual(g, color.G);
            Assert.AreEqual(b, color.B);

            #endregion
        }

        [TestMethod]
        public void RgbTextFailTest()
        {
            #region Arrange

            var threwException = false;

            #endregion

            #region Act

            try
            {
                var invalidRgbStr = "rgb(1234, .2341%, 1234)";
                var color = TextHelper.FromRgbString(invalidRgbStr);
            }
            catch (Exception)
            {
                threwException = true;
            }

            #endregion

            #region Assert

            Assert.IsTrue(threwException);

            #endregion
        }
    }
}
