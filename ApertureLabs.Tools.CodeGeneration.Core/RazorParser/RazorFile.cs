using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.RazorParser
{
    public static class ProjectFileExtensions
    {
        public static Project AddRazorFiles(this Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            // Retrieve all razor files in project folder.
            var projectDirectory = new FileInfo(project.FilePath)
                .Directory
                .FullName;
            var razorFiles = Directory.GetFiles(
                projectDirectory,
                "*.cshtml",
                SearchOption.AllDirectories);

            var modifiedProject = project;

            foreach (var file in razorFiles)
            {
                var stream = File.OpenRead(file);

                // Retrieve the folders.
                var relativePath = new FileInfo(file)
                    .Directory
                    .FullName
                    .Replace(
                        projectDirectory,
                        String.Empty,
                        StringComparison.Ordinal);

                var seperator = relativePath.Contains(Path.DirectorySeparatorChar, StringComparison.Ordinal)
                    ? Path.DirectorySeparatorChar
                    : Path.AltDirectorySeparatorChar;

                var folders = relativePath.Split(
                    seperator,
                    StringSplitOptions.RemoveEmptyEntries);

                //modifiedProject = modifiedProject.AddAdditionalDocument(
                modifiedProject = modifiedProject.AddDocument(
                        name: Path.GetFileName(file),
                        text: SourceText.From(stream),
                        folders: folders,
                        filePath: file)
                    .Project;
            }

            return modifiedProject;
        }
    }

    //public class RazorFile : TextDocument
    //{
    //    #region Fields

    //    private WeakReference<SemanticModel> model;

    //    #endregion

    //    #region Constructor

    //    internal RazorFile(Project project,
    //        DocumentId documentId,
    //        string filePath)
    //    {
    //        if (project == null)
    //            throw new ArgumentNullException(nameof(project));
    //        else if (String.IsNullOrEmpty(filePath))
    //            throw new ArgumentNullException(nameof(project));

    //        FilePath = filePath;
    //    }

    //    #endregion

    //    #region Methods

    //    public async Task<SemanticModel> GetSemanticModelAsync(
    //        CancellationToken token = default)
    //    {
    //        var syntaxTree = await GetSyntaxTreeAsync(token).ConfigureAwait(false);
    //        var compilation = await Project.GetCompilationAsync(token).ConfigureAwait(false);

    //        var semanticModel = default(SemanticModel);
    //        var result = compilation.GetSemanticModel(syntaxTree);

    //        if (result == null)
    //            throw new Exception();

    //        var original = Interlocked.CompareExchange(
    //            location1: ref model,
    //            value: new WeakReference<SemanticModel>(result),
    //            comparand: null);

    //        if (original == null)
    //            return result;

    //        lock (original)
    //        {
    //            if (original.TryGetTarget(out semanticModel))
    //            {
    //                return semanticModel;
    //            }

    //            original.SetTarget(result);
    //            return result;
    //        }

    //        throw new NotImplementedException();
    //    }

    //    public Task<SyntaxTree> GetSyntaxTreeAsync(CancellationToken cancellationToken)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
}
