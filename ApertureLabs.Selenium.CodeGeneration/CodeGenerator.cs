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
        /// Generates the specified original document.
        /// </summary>
        /// <param name="originalDocument">The original document.</param>
        /// <param name="destinationDocument">The destination document.</param>
        /// <param name="metadata">The metadata.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public abstract Task<Document> Generate(
            Document originalDocument,
            Document destinationDocument,
            IDictionary<string, object> metadata,
            IProgress<CodeGenerationProgress> progress,
            CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a list of all files that will be 'generated'. If needed
        /// document generation is allowed here ONLY in the destination
        /// project. Do NOT modify the orginal project!
        /// </summary>
        /// <param name="originalProject">The original project.</param>
        /// <param name="destinationProject">The destination project.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public abstract Task<IEnumerable<CodeGenerationContext>> GetContexts(
            Project originalProject,
            Project destinationProject,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets or creates the document with same relative path as the
        /// template document.
        /// </summary>
        /// <param name="templateDocument">The template document.</param>
        /// <param name="destinationProject">The destination project.</param>
        /// <param name="newFileName">New name of the file.</param>
        /// <returns></returns>
        protected Document GetOrCreateDocumentWithSameRelativePath(
            Document templateDocument,
            Project destinationProject,
            string newFileName)
        {
            var folders = GetRelativeFolders(templateDocument);
            var destinationDocument = default(Document);

            // Try and retrieve any existing documents that match.
            foreach (var document in destinationProject.Documents)
            {
                var docFolders = GetRelativeFolders(document);

                if (Enumerable.SequenceEqual(folders, docFolders)
                    && document.Name.Equals(newFileName, StringComparison.Ordinal))
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

            return destinationDocument;
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
            Document templateDocument)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));
            else if (templateDocument == null)
                throw new ArgumentNullException(nameof(templateDocument));
            else if (String.IsNullOrEmpty(newFileName))
                throw new ArgumentNullException(nameof(newFileName));

            var relativePath = GetLocalPathToFile(
                templateDocument.Project.FilePath,
                templateDocument.FilePath);

            var folders = GetFolders(relativePath);

            var addedDocument = project.AddDocument(
                name: newFileName,
                text: String.Empty,
                folders: folders);

            // Apply changes.
            var applyResult = addedDocument.Project.Solution.Workspace
                .TryApplyChanges(project.Solution);

            if (!applyResult)
                throw new Exception("Failed to update the project.");

            project = addedDocument.Project;

            // Return the modified project.
            return addedDocument;
        }

        /// <summary>
        /// Gets the documents with file extensions. Uses
        /// <see cref="Path.GetExtension(String)"/> to get the extension from
        /// the documents filepath.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fileExtensions">
        /// The file extensions. Should be prefixed with '.'
        /// </param>
        /// <returns></returns>
        protected IEnumerable<Document> GetDocumentsWithFileExtensions(
            Project project,
            IEnumerable<string> fileExtensions)
        {
            foreach (var document in project.Documents)
            {
                var extension = Path.GetExtension(document.FilePath);

                if (fileExtensions.Contains(extension))
                    yield return document;
            }
        }

        private string GetLocalPathToFile(
            string projectPath,
            string pathToFile)
        {
            var projectUri = new Uri(projectPath);
            var fileUri = new Uri(pathToFile);
            var relativeUri = fileUri.MakeRelativeUri(projectUri).ToString();

            // Need to exclude the base folder.
            var seperator = relativeUri.Contains(Path.DirectorySeparatorChar)
                ? Path.DirectorySeparatorChar
                : Path.AltDirectorySeparatorChar;

            var cleanedRelativePath = relativeUri.Remove(
                0,
                relativeUri.IndexOf(seperator));

            return cleanedRelativePath;
        }

        /// <summary>
        /// Gets the folders of the path except for the last entry.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string[] GetFolders(string path)
        {
            var seperator = path.Contains(Path.DirectorySeparatorChar)
                ? Path.DirectorySeparatorChar
                : Path.AltDirectorySeparatorChar;

            var folders = path.Split(seperator);

            return folders
                .Except(new[] { folders.LastOrDefault() })
                .ToArray();
        }

        private string[] GetRelativeFolders(Document document)
        {
            var localPath = GetLocalPathToFile(
                document.Name,
                document.Project.FilePath);

            return GetFolders(localPath);
        }
    }
}
