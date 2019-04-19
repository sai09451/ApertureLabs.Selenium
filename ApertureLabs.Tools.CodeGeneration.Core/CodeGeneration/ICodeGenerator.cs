using Microsoft.CodeAnalysis;
using System;
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
        /// Generates/updates the destination project with based on the
        /// original project. Do NOT call MSBuildWorkSpace.TryApplyChanges on
        /// either projects.
        /// </summary>
        /// <param name="originalProject">
        /// The original project. Changes to this project will be ignored.
        /// </param>
        /// <param name="destinationProject">
        /// The destination project.
        /// </param>
        /// <param name="progress">
        /// The update callback for signalling progress.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>The modified destination project.</returns>
        Task<Project> Generate(
            Project originalProject,
            Project destinationProject,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken token);
    }
}
