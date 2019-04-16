using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.RazorParser;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Services
{
    public class SeleniumCodeGenerator : CodeGenerator
    {
        public SeleniumCodeGenerator()
        { }

        public static string GetNestedPartialView(IntermediateNode node)
        {
            var content = GetContentOfNode(node);

            // Check if async partial tag helper.
            var match = Regex.Match(
                content,
                "^await Html\\.PartialAsync\\(\"(.*?)\"\\)$");

            if (match.Success)
                return match.Groups[1].Value;

            // Check if sync partial tag helper.
            match = Regex.Match(
                content,
                "^Html\\.Partial\\(\"(.*?)\"\\)$");

            if (match.Success)
                return match.Groups[1].Value;

            // Check if html partial tag helper.
            match = Regex.Match(
                content,
                "<partial[^>]*?name=[\"'](.+)[\"'].*?\\/>",
                RegexOptions.Multiline);

            if (match.Success)
                return match.Groups[1].Value;

            return null;
        }

        public static string GetLayoutOrDefault(IEnumerable<IntermediateNode> nodes)
        {
            foreach (var node in nodes)
            {
                var layout = GetLayoutOrDefault(node);

                if (String.IsNullOrEmpty(layout))
                    continue;

                return layout;
            }

            return null;
        }

        public static string GetLayoutOrDefault(IntermediateNode node)
        {
            IEnumerable<CSharpCodeIntermediateNode> allNodes;

            switch (node)
            {
                case CSharpCodeIntermediateNode csharpNode:
                    allNodes = new[] { csharpNode };
                    break;
                default:
                    allNodes = node.FindDescendantNodes<CSharpCodeIntermediateNode>();
                    break;
            }

            foreach (var n in allNodes)
            {
                var tokens = n.Children.OfType<IntermediateToken>();

                foreach (var token in tokens)
                {
                    var trimmedContent = token.Content.Trim();
                    var reader = new StringReader(trimmedContent);

                    for (var line = reader.ReadLine(); line != null; /*Empty*/ )
                    {
                        var match = Regex.Match(line, @"^Layout\s*?=\s*?(.\S*);$");

                        if (!match.Success)
                            continue;

                        var layout = match.Groups[1].Value;

                        return layout;
                    }
                }
            }

            return null;
        }

        public static bool IgnoreWhiteSpace(IntermediateNode node)
        {
            string content = node is IntermediateToken token
                ? token.Content
                : GetContentOfNode(node);

            // Try and find any non-whitespace characters.
            return Regex.IsMatch(content, @"\S");
        }

        public static IEnumerable<IReadOnlyList<IntermediateNode>> GroupChildNodes(
            IReadOnlyList<IntermediateNode> nodes)
        {
            var invalidNodes = new List<IntermediateNode>();

            for (var i = 0; i < nodes.Count; i++)
            {
                var previousNodes = nodes.Take(i - 1);
                var previousNode = previousNodes.FirstOrDefault();
                var childNode = nodes[i];
                var nextNodes = nodes.Skip(i).ToList();
                var nextNode = nextNodes.FirstOrDefault();

                if (childNode is CSharpCodeIntermediateNode csharpNode)
                {
                    // Check if directive or code block.
                }
                else if (childNode is HtmlContentIntermediateNode htmlNode)
                {

                }

                // Check if the list of tokens is good to return.
                if (true)
                {

                }

                // Check for invalid nodes.

                previousNode = childNode;
            }

            yield break;
        }

        public static string GetContentOfNodes(IEnumerable<IntermediateNode> nodes)
        {
            return String.Concat(nodes.Select(n => GetContentOfNode(n)));
        }

        public static string GetContentOfNode(IntermediateNode node)
        {
            var tokenNodes = node.FindDescendantNodes<IntermediateToken>();

            return String.Concat(tokenNodes.Select(n => n.Content));
        }

        public static bool IsWellFormedHtml(HtmlContentIntermediateNode node)
        {
            var htmlDoc = new HtmlDocument();
            var content = GetContentOfNode(node);
            htmlDoc.LoadHtml(content);

            return !htmlDoc.ParseErrors.Any();
        }

        public static bool IsNotableElement(HtmlContentIntermediateNode node)
        {
            // No child elements.
            if (HasChildElements(node))
                return false;

            // Must have an id attribute.

            return true;
        }

        public static bool HasChildElements(IntermediateNode node)
        {
            //var allChildNodes = node.FindDescendantNodes<IntermediateNode>();
            //allChildNodes.Where(n => n.)

            return true;
        }

        public static string GetIdOfElement(HtmlContentIntermediateNode node)
        {
            return String.Empty;
        }

        public override async Task<Project> Generate(
            Project originalProject,
            Project destinationProject,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken cancellationToken)
        {
            originalProject = originalProject.AddRazorFiles();
            var modifiedDestProj = destinationProject;

            var originalProjectDir = new FileInfo(destinationProject.FilePath)
                .Directory
                .FullName;

            var razorFiles = GetDocumentsWithFileExtensions(
                originalProject,
                new[] { ".cshtml" });

            var razorFileInfoList = new List<RazorPageInfo>();

            var razorEngine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(originalProjectDir));

            foreach (var razorFile in razorFiles)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Create or retrieve the destination document.
                var destinationName = razorFile.Name + "PageObjects";
                modifiedDestProj = GetOrCreateDocumentWithSameRelativePath(
                    razorFile,
                    modifiedDestProj,
                    destinationName,
                    out var generatedDocument,
                    out var relativePath);

                if (!(razorFile is Document razorFileDocument))
                    continue;

                var razorPageInfo = new RazorPageInfo
                {
                    PhysicalPath = razorFile.FilePath,
                    //GeneratedDocument = generatedDocument,
                    RelativePath = relativePath
                };

                razorFileInfoList.Add(razorPageInfo);
            }

            foreach (var razorFileInfo in razorFileInfoList)
            {
                var originalFilesText = File.ReadAllText(razorFileInfo.PhysicalPath);

                // Get the layout if any.
                var layoutMatch = Regex.Match(
                    originalFilesText,
                    "(?<=Layout\\s?=\\s?\")(.*)(?= \";)");

                // Get all included partial pages.
                var partialPagesAsyncHtmlMatch = Regex.Matches(
                    originalFilesText,
                    "(?<=await Html\\.PartialAsync\\(\")(.*?)(?= \")");

                var partialPagesSyncHtmlMatch = Regex.Matches(
                    originalFilesText,
                    "(?<=Html\\.Partial\\(\")(.*?)(?= \")");

                var partialPagesTagHelperMatch = Regex.Matches(
                    originalFilesText,
                    "(?<=<partial\\s.*?name=\")(.*?)(?= \")");

                var totalPartialPages = partialPagesAsyncHtmlMatch.Count
                    + partialPagesSyncHtmlMatch.Count
                    + partialPagesTagHelperMatch.Count;

                var allPartialPages = partialPagesAsyncHtmlMatch
                    .Concat(partialPagesSyncHtmlMatch)
                    .Concat(partialPagesTagHelperMatch);

                // Retrieve all Model properties being displayed.
                var allModelMembersMatch = Regex.Matches(
                    originalFilesText,
                    @"(?<=Model\.)([^\(\)<>\s]+)");

                // Determine if this is a viewcomponent.
                var isViewComponent = IsViewComponent(razorFileInfo.PhysicalPath);

                // Retrieve all viewcomponents.
                var viewCompInvokeAsyncMatches = Regex.Matches(
                    originalFilesText,
                    "(?<=await Component\\.InvokeAsync\\(\")(.*?)(?=\")");

                var viewCompTagHelperMatches = Regex.Matches(
                    originalFilesText,
                    @"(?<=<vc:)(.[^\s]*?)(?=\s)");

                var allViewComponentMatches = viewCompInvokeAsyncMatches
                    .Concat(viewCompTagHelperMatches);

                //var destSemanticModel = await razorFileInfo.GeneratedDocument
                //    .GetSemanticModelAsync(cancellationToken)
                //    .ConfigureAwait(false);

                //var destinationRootNode = destSemanticModel
                //    .SyntaxTree.GetRoot(cancellationToken);
            }

            return modifiedDestProj;
        }

        private bool IsViewComponent(string filePath)
        {
            var razorFileName = Path
                .GetFileNameWithoutExtension(filePath);

            return razorFileName.EndsWith(
                "ViewComponent",
                StringComparison.Ordinal);
        }
    }
}