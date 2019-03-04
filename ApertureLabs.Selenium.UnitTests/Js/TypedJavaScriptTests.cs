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
using System.Reflection;

namespace ApertureLabs.Selenium.Js
{
    [TestClass]
    public class TypedJavaScriptTests
    {
        #region Fields

        private static IPageObjectFactory pageObjectFactory;
        private static IWebDriver driver;
        private static WebDriverFactory webDriverFactory;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Cleanup

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();
            driver = webDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddSingleton(driver)
                .AddSingleton(new PageOptions { Url = Startup.ServerUrl });

            pageObjectFactory = new PageObjectFactory(serviceCollection);
            pageObjectFactory.PreparePage<HomePage>();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            webDriverFactory.Dispose();
        }

        #endregion

        #region Tests

        [ExecuteJavaScriptTestData]
        [ServerRequired]
        [TestMethod]
        public void ExecuteJavaScriptTest(string script,
            JavaScriptValue[] arguments,
            JavaScriptType expectedType)
        {
            var executor = new TypedJavaScriptExecutor(
                driver.JavaScriptExecutor());

            var result = executor.ExecuteJavaScript(
                script,
                arguments);

            object castedResult = null;

            if (expectedType == JavaScriptType.Boolean)
                castedResult = result.ToBool();
            else if (expectedType == JavaScriptType.BooleanArray)
                castedResult = result.ToBoolArray();
            else if (expectedType == JavaScriptType.Number)
                castedResult = result.ToNumber();
            else if (expectedType == JavaScriptType.NumberArray)
                castedResult = result.ToNumberArray();
            else if (expectedType == JavaScriptType.String)
                castedResult = result.ToString();
            else if (expectedType == JavaScriptType.StringArray)
                castedResult = result.ToStringArray();
            else if (expectedType == JavaScriptType.WebElement)
                castedResult = result.ToWebElement();
            else if (expectedType == JavaScriptType.WebElementArray)
                castedResult = result.ToWebElementArray();

            Assert.AreEqual(result.GetArgumentType(), expectedType);

            if (expectedType != JavaScriptType.Null)
                Assert.IsNotNull(castedResult);
            else
                Assert.IsNull(castedResult);
        }

        #endregion

        #region Nested Classes

        private class ExecuteJavaScriptTestData : Attribute, ITestDataSource
        {
            private int iteration;

            public ExecuteJavaScriptTestData()
            {
                iteration = 0;
            }

            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                var frameworkElement = driver.FindElement(
                    By.CssSelector(
                        "#framework-Bootstrap"));

                yield return new object[]
                {
                    "return null;",
                    Array.Empty<JavaScriptValue>(),
                    JavaScriptType.Null
                };

                yield return new object[]
                {
                    "return false",
                    Array.Empty<JavaScriptValue>(),
                    JavaScriptType.Boolean
                };

                yield return new object[]
                {
                    "return [ false, true, false ]",
                    Array.Empty<JavaScriptValue>(),
                    JavaScriptType.BooleanArray
                };

                yield return new object[]
                {
                    "return 'testing 1 2 3'",
                    Array.Empty<JavaScriptValue>(),
                    JavaScriptType.String
                };

                yield return new object[]
                {
                    "return ['test', '1 2 3', 'a b c'];",
                    Array.Empty<JavaScriptValue>(),
                    JavaScriptType.StringArray
                };

                yield return new object[]
                {
                    "return arguments[0];",
                    new[] { new JavaScriptValue(frameworkElement) },
                    JavaScriptType.WebElement
                };

                yield return new object[]
                {
                    "return [arguments[0]];",
                    new[] { new JavaScriptValue(frameworkElement) },
                    JavaScriptType.WebElementArray
                };
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                var name = $"Iteration {++iteration} - {methodInfo.Name}";

                if (data?.Any() ?? false)
                {
                    var dataNames = data
                        .Select(d => new { Name = d.GetType().Name, Value = d.ToString() })
                        .Select(d => $"{d.Name}: {d.Value}")
                        .ToList();

                    name += $" ({String.Join(", ", dataNames)}";
                }

                return name;
            }
        }

        #endregion
    }
}
