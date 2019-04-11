using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.RazorParser;
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
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Services
{
    public class SeleniumCodeGenerator : CodeGenerator
    {
        public SeleniumCodeGenerator()
        { }

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

            var razorFiles = GetDocumentsWithFileExtensions(
                originalProject,
                new[] { ".cshtml" });

            var modifiedDestProj = destinationProject;

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

                var originalRootNode = originalSemanticModel.SyntaxTree.GetRoot(cancellationToken);
                var destinationRootNode = destSemanticModel.SyntaxTree.GetRoot(cancellationToken);

                // Log info.
                LogInfo(razorFile.Name, originalSemanticModel, engine);
            }

            return modifiedDestProj;
        }

        private void LogInfo(string fileName,
            SemanticModel semanticModel,
            RazorProjectEngine razorProjectEngine)
        {
            Program.Log.Info("Info of " + fileName);

            var rootNode = semanticModel.SyntaxTree.GetRoot();
            var typeInfo = semanticModel.GetTypeInfo(rootNode);

            var projectItem = razorProjectEngine.FileSystem.GetItem(
                semanticModel.SyntaxTree.FilePath);

            var document = razorProjectEngine.Process(projectItem);
            var tree = document.GetSyntaxTree();
            var rootObj = tree.GetPropertyReflection<object>("Root");
            var proxyNode = new ProxySyntaxTreeNode(rootObj);

            LogProxyNode(proxyNode);
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