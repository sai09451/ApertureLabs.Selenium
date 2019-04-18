using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Base class for code generators. Provides methods for adding a file to
    /// a project.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.CodeGeneration.ICodeGenerator" />
    public abstract class CodeGenerator : ICodeGenerator
    {
        /// <summary>
        /// Retrieves or creates the file from the destination project. If
        /// creating then the file will be located in the same folder path
        /// as the templateDocument of the destinationProject directory but
        /// will use the new file name instead.
        /// </summary>
        /// <param name="templateDocument"></param>
        /// <param name="destinationProject"></param>
        /// <param name="newFileName"></param>
        /// <param name="destinationDocument"></param>
        /// <param name="relativePath"></param>
        /// <returns>The modified destination project</returns>
        protected Project GetOrCreateDocumentWithSameRelativePath(
            TextDocument templateDocument,
            Project destinationProject,
            string newFileName,
            out Document destinationDocument,
            out string relativePath)
        {
            destinationDocument = null;
            var folders = templateDocument.Folders;

            // Try and retrieve any existing documents that match.
            foreach (var document in destinationProject.Documents)
            {
                var documentExists = Enumerable.SequenceEqual(folders, document.Folders)
                    && document.Name.Equals(
                        Path.ChangeExtension(newFileName, ".cs"),
                        StringComparison.Ordinal);

                if (documentExists)
                {
                    destinationDocument = document;
                    break;
                }
            }

            // Create the document if not found.
            if (destinationDocument == null)
            {
                destinationDocument = AddFileToProject(
                    destinationProject,
                    newFileName,
                    templateDocument);
            }

            relativePath = Path.Combine(folders
                .Concat(new[] { newFileName })
                .ToArray());

            return destinationDocument.Project;
        }

        /// <summary>
        /// Adds the file to project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="templateDocument">
        /// The template document. Will place the new file in the same folders
        /// as the templateDocument. Ex: template doc is ./a/b/c/template.cs
        /// then the added file will be ./a/b/c/newFileName.cs.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// project
        /// or
        /// templateDocument
        /// or
        /// newFileName
        /// </exception>
        /// <exception cref="Exception">Failed to update the project.</exception>
        protected Document AddFileToProject(Project project,
            string newFileName,
            TextDocument templateDocument)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            else if (templateDocument == null)
                throw new ArgumentNullException(nameof(templateDocument));
            else if (String.IsNullOrEmpty(newFileName))
                throw new ArgumentNullException(nameof(newFileName));

            var projectDir = new FileInfo(project.FilePath)
                .Directory
                .FullName;

            var filePath = Path.Combine(
                projectDir,
                Path.Combine(templateDocument.Folders.ToArray()),
                Path.ChangeExtension(newFileName, ".cs"));

            var addedDocument = project.AddDocument(
                name: newFileName,
                text: String.Empty,
                folders: templateDocument.Folders,
                filePath: filePath);

            // Return the modified project.
            return addedDocument;
        }

        /// <summary>
        /// Gets the documents with file extensions.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileExtensions">
        /// The file extensions. Should be prefixed with '.'
        /// </param>
        /// <returns></returns>
        protected IEnumerable<TextDocument> GetDocumentsWithFileExtensions(
            Project project,
            IEnumerable<string> fileExtensions)
        {
            var source = Enumerable.Concat(
                project.Documents,
                project.AdditionalDocuments);

            foreach (var document in source)
            {
                var hasExtension = fileExtensions.Any(
                    ext => document.FilePath.EndsWith(
                        ext,
                        StringComparison.Ordinal));

                if (hasExtension)
                    yield return document;
            }
        }

        protected string ConvertFoldersToNamesapce(IEnumerable<string> folders)
        {
            throw new NotImplementedException();
        }

        protected string ConvertNamespaceToFolders(string @namespace)
        {
            throw new NotImplementedException();
        }

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
        public abstract Task<Project> Generate(
            Project originalProject,
            Project destinationProject,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken token);
    }
}
