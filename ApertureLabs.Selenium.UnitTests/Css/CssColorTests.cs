using ApertureLabs.Selenium.Css;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ApertureLabs.Selenium.UnitTests.Css
{
    [TestClass]
    public class CssColorTests
    {
        /// <summary>
        /// All datarow parameters should be red with no transparency.
        /// </summary>
        /// <param name="hslString"></param>
        [DataTestMethod]
        [DataRow("hsl(0, 100%, 50%)")]
        [DataRow("rgb(255, 0, 0)")]
        [DataRow("#ff0000")]
        [DataRow("#f00")]
        public void CssColorTest(string hslString)
        {
            var cssColor = new CssColor(hslString);
            var c = cssColor.Color;

            Assert.AreEqual(cssColor.Color.A, Color.Red.A);
            Assert.AreEqual(cssColor.Color.R, Color.Red.R);
            Assert.AreEqual(cssColor.Color.G, Color.Red.G);
            Assert.AreEqual(cssColor.Color.B, Color.Red.B);
        }
    }
}
