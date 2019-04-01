using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class MappedFileModel
    {
        #region Constructor

        public MappedFileModel(Project project,
            ProjectItem projectItem,
            string selectedProjectPath)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var name = projectItem.Name;

            var projectPath = new Uri(
                new FileInfo(project.FullName).Directory.FullName);

            var fullPath = new Uri(projectItem.Properties
                ?.Item("FullPath")
                ?.Value
                ?.ToString());

            var relativePath = projectPath
                .MakeRelativeUri(fullPath)
                .ToString();

            var seperator = Path.DirectorySeparatorChar;

            if (!relativePath.Contains(seperator))
                seperator = Path.AltDirectorySeparatorChar;

            // This is to remove the 'base' dir of the relative path.
            relativePath = relativePath.Remove(
                0,
                relativePath.IndexOf(seperator) + 1);

            IsIgnored = false;
            IsNewFile = false;
            SelectedComponentTypeNameIndex = 0;
            RelativePathToFileFromProject = relativePath;
            FileName = projectItem.Name;
            ProjectItemReference = projectItem;

            UpdateProjectPath(relativePath);
        }

        #endregion

        #region Properties

        public bool IsNewFile { get; set; }
        public bool IsIgnored { get; set; }
        public string RelativePathToFileFromProject { get; set; }
        public string FileName { get; set; }
        public string NewPath { get; set; }
        public int SelectedComponentTypeNameIndex { get; set; }
        public ProjectItem ProjectItemReference { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the project path.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <exception cref="ArgumentNullException">project</exception>
        public void UpdateProjectPath(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var projectPath = project.GetProjectFolder();
            UpdateProjectPath(projectPath.ToString());
        }

        /// <summary>
        /// Updateds the project path.
        /// </summary>
        /// <param name="availableProjectModel">The available project model.</param>
        /// <exception cref="ArgumentNullException">availableProjectModel</exception>
        public void UpdateProjectPath(AvailableProjectModel availableProjectModel)
        {
            if (availableProjectModel == null)
                throw new ArgumentNullException(nameof(availableProjectModel));

            UpdateProjectPath(availableProjectModel.PathToProjectFolder);
        }

        private void UpdateProjectPath(string path)
        {
            var newPath = Path.ChangeExtension(
                    Path.GetFullPath(
                        Path.Combine(
                            path,
                            RelativePathToFileFromProject)),
                    ".cs");

            NewPath = newPath;
            IsNewFile = !File.Exists(newPath);
        }

        #endregion
    }
}
