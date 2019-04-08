using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Used be the ApertureLabs.Tools.CodeGeneration.Core tool to generate
    /// code files.
    /// </summary>
    public interface ICodeGenerator
    {
        /// <summary>
        /// Retrieves a list of all files that will be 'generated'. If needed
        /// document generation is allowed here ONLY in the destination
        /// project. Do NOT modify the orginal project!
        /// </summary>
        /// <param name="originalProject">The original project.</param>
        /// <param name="destinationProject">The destination project.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<CodeGenerationContext>> GetContexts(
            Project originalProject,
            Project destinationProject,
            CancellationToken cancellationToken);

        /// <summary>
        /// Called to generate the document. Do NOT apply changes in this
        /// function. Should return the modified document instead. Exceptions
        /// can be thrown in this function however the changes will NOT be
        /// applied.
        /// </summary>
        /// <param name="originalDocument">The original document.</param>
        /// <param name="destinationDocument">The destination document.</param>
        /// <param name="metadata">
        /// The metadata associated with the context.
        /// </param>
        /// <param name="progress">The progress handler.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A task whose result should be the modified/generated document.
        /// </returns>
        Task<Document> Generate(Document originalDocument,
            Document destinationDocument,
            IDictionary<string, object> metadata,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken cancellationToken);
    }
}
