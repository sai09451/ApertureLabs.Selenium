using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class MappedFileModel : INotifyPropertyChanged
    {
        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructor

        public MappedFileModel(ProjectItem projectItem,
            Uri projectPath,
            string selectedProjectPath,
            IReadOnlyList<string> availableComponentTypeNames)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            var name = projectItem.Name;

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

            // Calling Path.GetFullPath will normalize the path.
            var newFullPath = Path.ChangeExtension(
                Path.GetFullPath(
                    Path.Combine(
                        selectedProjectPath,
                        relativePath)),
                ".cs");

            IsIgnored = false;
            IsNewFile = false;
            SelectedComponentTypeNameIndex = 0;
            NewPath = newFullPath;
            OriginalPathRelativeToProject = relativePath;
            FileName = projectItem.Name;
            ProjectItemReference = projectItem;
            AvailableComponentTypeNames = availableComponentTypeNames;
        }

        #endregion

        #region Properties

        public bool IsNewFile { get; set; }
        public bool IsIgnored { get; set; }
        public string OriginalPathRelativeToProject { get; set; }
        public string FileName { get; set; }
        public string NewPath { get; set; }
        public int SelectedComponentTypeNameIndex { get; set; }
        public ProjectItem ProjectItemReference { get; set; }
        public IReadOnlyList<string> AvailableComponentTypeNames { get; set; }

        #endregion

        #region Methods

        private void RaisePropertyChange([CallerMemberName]string propertyName = null)
        {
            if (String.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
