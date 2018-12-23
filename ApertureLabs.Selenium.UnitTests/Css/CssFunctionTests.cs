using ApertureLabs.Selenium.Css;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests.Css
{
    [TestClass]
    public class CssFunctionTests
    {
        [DataTestMethod]
        [DataRow("hsl(0, 100%, 50%")]
        public void CssFunctionTest(string cssFunctionString)
        {
            var cssFunction = new CssFunction(cssFunctionString);

            Assert.IsNotNull(cssFunction);
        }

        [DataTestMethod]
        [DataRow("hsl(0, 100%, 50%)", "hsl")]
        [DataRow("rgb(255, 255, 255)", "rgb")]
        [DataRow("rgba(255, 255, 255)", "rgba")]
        public void GetFunctionNameTest(string cssFunctionString, string name)
        {
            var cssFunction = new CssFunction(cssFunctionString);
            var funcName = cssFunction.FunctionName;

            Assert.AreEqual(funcName, name);
        }

        [DataTestMethod]
        [DataRow("hsl(0, 100%, 50%)", 3)]
        [DataRow("rgb(255, 255, 255)", 3)]
        [DataRow("rgba(255, 255, 255, 1)", 4)]
        public void GetFunctionArguments(string cssFunc, int numberOfArgs)
        {
            var cssFunction = new CssFunction(cssFunc);
            var args = cssFunction.Arguments;

            Assert.AreEqual(args.Count, numberOfArgs);
        }
    }
}
