using ApertureLabs.Tools.CodeGeneration.Core.Services;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Tests.Services
{
    [TestClass]
    public class SeleniumCodeGeneratorTests
    {
        public TestContext TestContext { get; set; }

        private const string PATH_TO_RAZOR_PROJECT = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\MockServer.csproj";
        private const string PATH_TO_RAZOR_FILE = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\Pages\kendo\2014.1.318\KGrid.cshtml";

        private const string RAZOR_DEMO_1 =
            "<somexml>" +
                "@Model.Name" +
            "</somexml>";

        private const string RAZOR_DEMO_2 =
            "<somexml class=\"@Model.Classes\">" +
                "@Model.Name" +
            "</somexml>";

        private const string RAZOR_DEMO_3 =
            "<somexml>" +
                "@(String.IsNullOrEmpty(Model.Name))" +
            "</somexml>";

        private const string RAZOR_DEMO_4 =
            "@page\n" +
            "@Model Namespace.NestedNamespace.YourModelType\n" +
            "<somexml>\n" +
                "@foreach (var item in Model.Items)\n" +
                "{\n" +
                    "<div>@item.Name</div>\n" +
                "}\n" +
            "</somexml>";

        private static RazorProjectEngine razorProjectEngine;
        private static RazorProjectItem razorProjectItem;
        private static RazorCodeDocument razorCodeDocument;
        private static RazorSyntaxTree syntaxTree;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var projectDir = new FileInfo(PATH_TO_RAZOR_PROJECT)
                .Directory
                .FullName;

            RazorProjectFileSystem fs = RazorProjectFileSystem
                .Create(projectDir);

            razorProjectEngine = RazorProjectEngine.Create(
                configuration: RazorConfiguration.Default,
                fileSystem: fs);

            razorProjectItem = razorProjectEngine.FileSystem
                .GetItem(PATH_TO_RAZOR_FILE);

            razorCodeDocument = razorProjectEngine.Process(razorProjectItem);

            syntaxTree = razorCodeDocument.GetSyntaxTree();
        }

        [TestMethod]
        public void RazorTest()
        {
            var processedView = razorProjectEngine.ProcessDesignTime(razorProjectItem);
            var designTimeSyntaxTree = processedView.GetSyntaxTree();

            var designTimeAllNodes = processedView
                .GetDocumentIntermediateNode()
                .FindPrimaryMethod()
                .Children;

            var allNodes = razorCodeDocument
                .GetDocumentIntermediateNode()
                .FindPrimaryMethod()
                .Children;
            var importantNodes = allNodes
                .Where(SeleniumCodeGenerator.IgnoreWhiteSpace)
                .ToList();

            var allContent = SeleniumCodeGenerator.GetContentOfNodes(allNodes);
            var nodeGroups = SeleniumCodeGenerator.GroupChildNodes(importantNodes);

            var layout = SeleniumCodeGenerator.GetLayoutOrDefault(importantNodes);

            var partialNodes = importantNodes
                .Where(n => null != SeleniumCodeGenerator.GetNestedPartialView(n))
                .ToList();

            StringAssert.Equals(
                "~/Pages/kendo/2014.1.318/Shared/_Layout.cshtml",
                layout);
        }

        [TestMethod]
        public void RazorParseTest1()
        {
            var cleanedHtml = Regex.Replace(
                RAZOR_DEMO_1,
                @"(?<!\s)@(?!foreach)(?!model)(?!using)(?!page)(?!\()([^<\/\s]*)",
                ImplicitEvaluator);

            //cleanedHtml = Regex.Replace(
            //    cleanedHtml,
            //    @"",
            //    ImplicitEvaluator);

            Assert.IsFalse(String.IsNullOrEmpty(cleanedHtml));
        }

        private string ImplicitEvaluator(Match match)
        {
            var xelement = new XElement(
                "csharp-implicit",
                new XAttribute("evaluate", match.Value));

            return xelement.ToString(SaveOptions.DisableFormatting);
        }

        private string DirectiveEvaluator(Match match)
        {
            var xelement = new XElement(
                "csharp-directive",
                new XAttribute("evaluate", match.Value));

            return xelement.ToString(SaveOptions.DisableFormatting);
        }

        private string ControlEvaluator(Match match)
        {
            throw new NotImplementedException();
        }
    }
}
