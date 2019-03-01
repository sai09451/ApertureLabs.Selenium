using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Just verifying the UriTemplate class works as expected.
    /// </summary>
    [TestClass]
    public class UriTemplateTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [UriTemplateTest1Data]
        public void UriTemplateTest1(Uri baseUri,
            UriTemplate template,
            IEnumerable<string> parameters = null)
        {
            var @params = new Dictionary<string, string>();

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    var keyValue = p.Split('=', 2);
                    @params.Add(keyValue[0], keyValue[1]);
                }
            }

            var uri = template.BindByName(baseUri, @params);

            Assert.IsNotNull(template);
            Assert.IsNotNull(uri);
        }

        [TestMethod]
        public void UriTest()
        {
            var uri = new Uri(".", UriKind.Relative);
        }

        #region Nested Classes

        /// <summary>
        /// Helper class for the UriTemplateTest1 test.
        /// </summary>
        /// <seealso cref="System.Attribute" />
        /// <seealso cref="Microsoft.VisualStudio.TestTools.UnitTesting.ITestDataSource" />
        class UriTemplateTest1DataAttribute : Attribute, ITestDataSource
        {
            private int iteration = 0;

            public IEnumerable<object[]> GetData(MethodInfo methodInfo)
            {
                yield return new object[]
                {
                    new Uri("https://www.google.com/"),
                    new UriTemplate("{q=test}"),
                    new[] { "q=someValue" }
                };

                yield return new object[]
                {
                    new Uri("https://www.google.com/"),
                    new UriTemplate("{q}/{id}/{blah=something}"),
                    new[] { "q=anotherValue", "id=4" }
                };

                yield return new object[]
                {
                    new Uri("https://www.google.com/"),
                    new UriTemplate(""),
                    null
                };
            }

            public string GetDisplayName(MethodInfo methodInfo, object[] data)
            {
                var pNames = methodInfo
                    .GetParameters()
                    .Select(p => p.Name)
                    .ToList();

                var dataStr = String.Join(
                    ", ",
                    data.Select((d, i) => $"{pNames[i]}: {d?.ToString() ?? String.Empty}"));

                return $"Iteration {++iteration}: #{methodInfo.Name}({dataStr})";
            }
        }

        #endregion
    }
}
