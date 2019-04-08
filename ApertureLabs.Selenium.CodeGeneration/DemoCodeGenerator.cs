using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.CodeGeneration
{
    public abstract class DemoCodeGenerator : CodeGenerator
    {
        public override async Task<Document> Generate(
            Document originalDocument,
            Document destinationDocument,
            IDictionary<string, object> metadata,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken cancellationToken)
        {
            if (originalDocument == null)
                throw new ArgumentNullException(nameof(originalDocument));
            else if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            var orginalSemanticModel = await originalDocument
                .GetSemanticModelAsync(cancellationToken);

            var destinationSemanticModel = await destinationDocument
                .GetSemanticModelAsync(cancellationToken);

            if (orginalSemanticModel == null)
            {
                throw new Exception("Failed to retrieve the semantic model " +
                    "of the original file.");
            }
            else if (destinationSemanticModel == null)
            {
                throw new Exception("Failed to retrieve the semantic model " +
                    "of the destination file.");
            }

            // Only handle C# for now.
            if (destinationSemanticModel.Language != LanguageNames.CSharp)
            {
                throw new Exception("Can only generate C# files for now.");
            }

            return destinationDocument.WithText(
                SourceText.From(
                    "",
                    Encoding.UTF8));
        }

        public override async Task<IEnumerable<CodeGenerationContext>> GetContexts(
            Project originalProject,
            Project destinationProject,
            CancellationToken cancellationToken)
        {
            var projectPath = new FileInfo(originalProject.FilePath)
                .Directory
                .FullName;

            var results = new List<CodeGenerationContext>();

            foreach (var document in originalProject.Documents)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var extension = Path.GetExtension(document.FilePath);

                if (!extension.Equals(".cshtml", StringComparison.Ordinal))
                    continue;

                // Create or retrieve the destination document.
                var destinationName = document.Name + "PageObjects";
                var destinationDoc = GetOrCreateDocumentWithSameRelativePath(
                    document,
                    destinationProject,
                    destinationName);

                results.Add(new CodeGenerationContext
                {
                    DestinationDocumentId = destinationDoc.Id,
                    OriginalDocumentId = document.Id
                });
            }

            return results;
        }
    }
}
