using ApertureLabs.Tools.CodeGeneration.Core.RazorParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Tests.RazorParser
{
    [TestClass]
    public class RazorFileTest
    {
        #region Fields

        private const string PATH_TO_RAZOR_FILE = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\Pages\Bootstrap\4.1\Collapsable.cshtml";
        private const string PATH_TO_DEMO_XML = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\ApertureLabs.Tools.CodeGeneration.Core.Tests\DemoXmlFile.xml";

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup

        #endregion

        #region Tests

        [TestMethod]
        public void RazorDocumentTest()
        {
            var razorDoc = new RazorDocument(PATH_TO_RAZOR_FILE);
        }

        [TestMethod]
        public void XmlReaderTest()
        {
            var stream = File.OpenRead(PATH_TO_DEMO_XML);
            using (var reader = XmlReader.Create(input: stream))
            {
                while (reader.Read())
                {
                    TestContext.WriteLine("Node info:");
                    LogInfo(() => reader.NodeType, 1);
                    LogInfo(() => reader.Name, 1);
                    LogInfo(() => reader.Depth, 1);
                    LogInfo(() => reader.IsEmptyElement, 1);
                    LogInfo(() => reader.HasValue, 1);
                    LogInfo(() => reader.ValueType, 1);

                    if (reader.HasAttributes)
                    {
                        TestContext.WriteLine("\tAttributes:");

                        while (reader.MoveToNextAttribute())
                        {
                            LogInfo(
                                expression: () => reader.Value,
                                indentation: 2,
                                prefix: reader.Name);
                        }

                        // Moves the reader back to the element.
                        reader.MoveToElement();
                    }
                }
            }
        }

        private void LogInfo<T>(Expression<Func<T>> expression,
            int indentation = 0,
            string prefix = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var result = expression.Compile().Invoke();

            if (String.IsNullOrEmpty(prefix))
            {
                switch (expression.Body)
                {
                    case MethodCallExpression methodCall:
                        prefix = methodCall.Method.Name;
                        break;
                    case MemberExpression member:
                        prefix = member.Member.Name;
                        break;
                }
            }

            var sb = new StringBuilder();

            if (indentation > 0)
                sb.Append('\t', indentation);

            sb.Append($"{prefix}: {result?.ToString() ?? "null"}");

            TestContext.WriteLine(sb.ToString());
        }

        #endregion
    }
}
