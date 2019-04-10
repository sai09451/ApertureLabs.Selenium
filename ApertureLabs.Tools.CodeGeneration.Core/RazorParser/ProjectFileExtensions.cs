using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;

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
}
