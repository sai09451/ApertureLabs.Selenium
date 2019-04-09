using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.RazorParser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
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
                LogInfo(razorFile.Name, originalSemanticModel);
            }

            return modifiedDestProj;
        }

        private void LogInfo(string fileName, SemanticModel semanticModel)
        {
            Program.Log.Info("Info of " + fileName);

            var rootNode = semanticModel.SyntaxTree.GetRoot();
            var typeInfo = semanticModel.GetTypeInfo(rootNode);

            Program.Log.Info($"\t*{typeInfo.Type.Name}");
        }

        private FileInfo GetRazorPageFile(Document document)
        {
            var razorFilePath = document.FilePath.Substring(
                0,
                document.FilePath.Length - 3);

            return new FileInfo(razorFilePath);
        }
    }
}