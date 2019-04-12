using ApertureLabs.Selenium.CodeGeneration;
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
            var projectDir = new FileInfo(originalProject.FilePath)
                .Directory
                .FullName;

            RazorProjectFileSystem fs = RazorProjectFileSystem
                .Create(projectDir);

            var engine = RazorProjectEngine.Create(
                configuration: RazorConfiguration.Default,
                fileSystem: fs);

            originalProject = originalProject.AddRazorFiles();
            var modifiedDestProj = destinationProject;

            var razorFiles = GetDocumentsWithFileExtensions(
                originalProject,
                new[] { ".cshtml" });

            var originalCompilation = await originalProject
                .GetCompilationAsync(cancellationToken)
                .ConfigureAwait(false);

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
                    out var generatedDocument);

                if (!(razorFile is Document razorFileDocument))
                    continue;

                var originalSemanticModel = await razorFileDocument
                    .GetSemanticModelAsync(cancellationToken)
                    .ConfigureAwait(false);

                var destSemanticModel = await generatedDocument
                    .GetSemanticModelAsync(cancellationToken)
                    .ConfigureAwait(false);

                var originalRootNode = originalSemanticModel.SyntaxTree
                    .GetRoot(cancellationToken);
                var destinationRootNode = destSemanticModel
                    .SyntaxTree.GetRoot(cancellationToken);
                var razorDocument = engine.FileSystem
                    .GetItem(originalRootNode.SyntaxTree.FilePath);

                var razorCodeDocument = engine.Process(razorDocument);

                var rootIntermediateNode = razorCodeDocument.GetDocumentIntermediateNode();

                var razorSyntaxTree = CreateSyntaxTreeNode(originalSemanticModel, engine);

                // Log info.
                LogInfo(razorFile.Name, razorSyntaxTree);
            }

            return modifiedDestProj;
        }

        private ProxySyntaxTreeNode CreateSyntaxTreeNode(
            SemanticModel semanticModel,
            RazorProjectEngine razorProjectEngine)
        {
            var projectItem = razorProjectEngine.FileSystem.GetItem(
                semanticModel.SyntaxTree.FilePath);

            var document = razorProjectEngine.Process(projectItem);
            var tree = document.GetSyntaxTree();
            var rootObj = tree.GetPropertyReflection<object>("Root");
            var proxyNode = new ProxySyntaxTreeNode(rootObj);

            return proxyNode;
        }

        private void LogInfo(string fileName,
            ProxySyntaxTreeNode proxySyntaxTreeNode)
        {
            Program.Log.Info("Info of " + fileName);
            LogProxyNode(proxySyntaxTreeNode);
        }

        private static void LogProxyNode(ProxySyntaxTreeNode proxyNode,
            StringBuilder sb,
            int margin)
        {
            sb = sb ?? new StringBuilder();

            var marginSB = new StringBuilder();

            if (margin > 0)
                marginSB.Append('\t', margin);

            var marginStr = marginSB.ToString();

            sb.AppendLine($"{marginStr}* Node:");
            sb.AppendLine($"{marginStr}\t* IsBlock: {proxyNode.IsBlock}");

            if (proxyNode.IsBlock)
            {
                sb.AppendLine($"{marginStr}\t* BlockKind: {proxyNode.BlockKind}");
            }
            else
            {
                sb.AppendLine($"{marginStr}\t* SpanKind: {proxyNode.SpanKind}");
                sb.AppendLine($"{marginStr}\t* SpanContent: {proxyNode.SpanContent}");
            }

            if (proxyNode.Children.Any())
            {
                Program.Log.Info($"{marginStr}\t* Children:");
                var newMargin = margin + 1;

                foreach (var childNode in proxyNode.Children)
                    LogProxyNode(childNode, sb, newMargin);
            }
        }

        private static void LogProxyNode(ProxySyntaxTreeNode proxyNode)
        {
            var sb = new StringBuilder();
            LogProxyNode(proxyNode, sb, 0);
            Program.Log.Info(sb.ToString());
        }

        private class ProxySyntaxTreeNode
        {
            private readonly WeakReference<object> originalObject;

            public ProxySyntaxTreeNode(object obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                originalObject = new WeakReference<object>(obj);

                var parent = obj.GetPropertyReflection<object>(nameof(Parent));

                Parent = parent != null ? new ProxySyntaxTreeNode(parent) : null;
                Length = obj.GetPropertyReflection<int>(nameof(Length));
                Start = obj.GetPropertyReflection<object>(nameof(Start));
                IsBlock = obj.GetPropertyReflection<bool>(nameof(IsBlock));

                var type = obj.GetType();

                if (type.FullName == "Microsoft.AspNetCore.Razor.Language.Legacy.Block")
                {
                    BlockKind = obj.GetPropertyReflection<object>("Type").ToString();
                }
                else if (type.FullName == "Microsoft.AspNetCore.Razor.Language.Legacy.Span")
                {
                    SpanKind = obj.GetPropertyReflection<object>("Kind").ToString();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            public ProxySyntaxTreeNode Parent { get; }

            public int Length { get; }

            public object Start { get; }

            public bool IsBlock { get; }

            public string NodeKind { get; }

            public IEnumerable<ProxySyntaxTreeNode> Children
            {
                get
                {
                    if (!IsBlock)
                        yield break;

                    if (!originalObject.TryGetTarget(out var originalObj))
                        yield break;

                    var children = originalObj.GetPropertyReflection<object>(nameof(Children));

                    if (children is IEnumerable childrenEnum)
                    {
                        foreach (var childItemObj in childrenEnum)
                        {
                            var childItem = new ProxySyntaxTreeNode(childItemObj);
                            yield return childItem;
                        }
                    }
                }
            }

            public string SpanKind { get; set; }

            public string SpanContent
            {
                get
                {
                    if (IsBlock)
                        return null;

                    if (!originalObject.TryGetTarget(out var originalObj))
                        return null;

                    return originalObj.GetPropertyReflection<string>("Content");
                }
            }

            public string BlockKind { get; set; }
        }
    }

    internal static class ReflectionUtilties
    {
        internal static T GetPropertyReflection<T>(this object obj, string propName)
        {
            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
            var prop = props.FirstOrDefault(
                    p => p.Name.Equals(propName, StringComparison.Ordinal));

            if (prop == null)
                throw new Exception("No such property.");

            var val = prop.GetValue(obj);

            return (T)val;
        }
    }
}