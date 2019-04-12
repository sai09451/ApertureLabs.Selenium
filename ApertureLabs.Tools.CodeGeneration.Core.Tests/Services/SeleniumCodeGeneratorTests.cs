using ApertureLabs.Tools.CodeGeneration.Core.Services;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace ApertureLabs.Tools.CodeGeneration.Core.Tests.Services
{
    [TestClass]
    public class SeleniumCodeGeneratorTests
    {
        public TestContext TestContext { get; set; }

        private const string PATH_TO_RAZOR_PROJECT = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\MockServer.csproj";
        private const string PATH_TO_RAZOR_FILE = @"C:\Users\Alexander\Documents\GitHub\ApertureLabs.Selenium\MockServer\Pages\kendo\2014.1.318\KGrid.cshtml";

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
    }
}
