using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class SynchronizePageObjectsModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly List<AvailableProjectModel> availableProjects;
        private readonly List<MappedFileModel> fileMap;

        private List<string> availableComponentTypeNames;
        private string pageObjectLibraryName;
        private bool useAreas;
        private int selectedProjectIndex;
        private string originalProjectName;
        private string defaultNamespace;

        #endregion

        #region Constructor

        public SynchronizePageObjectsModel(
            Project project,
            DTE dte,
            IReadOnlyList<string> availableComponentTypeNames,
            IVsSolution2 solutionService)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null)
                throw new ArgumentNullException(nameof(project));
            else if (dte == null)
                throw new ArgumentNullException(nameof(dte));
            else if (availableComponentTypeNames == null)
                throw new ArgumentNullException(nameof(availableComponentTypeNames));
            else if (solutionService == null)
                throw new ArgumentNullException(nameof(solutionService));

            availableComponentTypeNames = new List<string>
            {
                "PageObject",
                "IPageObject",
                "PageComponent",
                "IPageComponent"
            };
            availableProjects = new List<AvailableProjectModel>
            {
                new AvailableProjectModel
                {
                    IsNew = true,
                    DisplayName = "New",
                    FullPath = String.Empty,
                    UniqueName = String.Empty
                }
            };
            fileMap = new List<MappedFileModel>();
            useAreas = true;

            // Get the solution folder.
            var solutionDir = new FileInfo(dte.Solution.FullName)
                .Directory
                .FullName;

            var defaultProjectName = $"{project.Name}.PageObjects";
            DefaultNamespace = defaultProjectName;
            OriginalProjectName = project.Name;

            var newProject = AvailableProjects[0];
            AvailableComponentTypeNames = availableComponentTypeNames;

            newProject.FullPath = Path.Combine(
                solutionDir,
                defaultProjectName);

            var projects = solutionService.GetProjects();

            foreach (var p in projects)
            {
                AddAvailableProject(new AvailableProjectModel
                {
                    DisplayName = p.Name,
                    FullPath = p.FullName,
                    IsNew = false,
                    UniqueName = p.UniqueName,
                });
            }

            // Check if a default project already exists.
            SelectedProjectIndex = AvailableProjects
                .Select((m, i) => new { Model = m, Index = i })
                .FirstOrDefault(m => m.Model.DisplayName == defaultProjectName)
                ?.Index
                ?? 0;

            // Retrieve the project folder path.
            var projectPath = new Uri(
                new FileInfo(project.FullName).Directory.FullName);

            var selectedProjectPath = SelectedProject.FullPath;

            // Now locate all razor files in the selected project.
            foreach (var item in project.GetAllProjectItems())
            {
                var mappedFile = new MappedFileModel(item,
                    projectPath,
                    selectedProjectPath,
                    availableComponentTypeNames);

                AddMappedFile(mappedFile);
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public IReadOnlyList<AvailableProjectModel> AvailableProjects => availableProjects.AsReadOnly();

        public IReadOnlyList<MappedFileModel> FileMap => fileMap.AsReadOnly();

        public string PageObjectLibraryName
        {
            get => pageObjectLibraryName;
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;
                else if (String.Equals(pageObjectLibraryName, value, StringComparison.Ordinal))
                    return;

                pageObjectLibraryName = value;
                RaisePropertyChange();
            }
        }

        public bool UseAreas
        {
            get => useAreas;
            set
            {
                if (useAreas == value)
                    return;

                useAreas = value;
                RaisePropertyChange();
            }
        }

        public int SelectedProjectIndex
        {
            get => selectedProjectIndex;
            set
            {
                if (selectedProjectIndex == value)
                    return;

                var selectedProj = SelectedProject;

                // Updated all file mappings.
                foreach (var fileMap in FileMap)
                {
                    var newPath = Path.GetFullPath(
                        Path.Combine(
                            selectedProj.FullPath,
                            fileMap.OriginalPathRelativeToProject,
                            fileMap.FileName));

                    fileMap.NewPath = newPath;
                }

                selectedProjectIndex = value;
                RaisePropertyChange();
                RaisePropertyChange(nameof(FileMap));
                RaisePropertyChange(nameof(SelectedProject));
            }
        }

        public AvailableProjectModel SelectedProject => AvailableProjects[SelectedProjectIndex];

        public string OriginalProjectName
        {
            get => originalProjectName;
            set
            {
                if (String.IsNullOrEmpty(value))
                    return;

                originalProjectName = value;
            }
        }

        public string DefaultNamespace
        {
            get => defaultNamespace;
            set
            {
                defaultNamespace = value;
                RaisePropertyChange();
            }
        }

        public EnvDTE.DTE DTE { get; set; }

        public IReadOnlyList<string> AvailableComponentTypeNames
        {
            get => availableComponentTypeNames;
            set
            {
                availableComponentTypeNames = (value ?? new List<string>()).ToList();
                RaisePropertyChange();
            }
        }

        #endregion

        #region Methods

        public Task StartOperationAsync()
        {
            // TODO
            return Task.CompletedTask;
        }

        public void OverridePath(string originalFilePath, string outputPath)
        {
            var mappedFile = GetFile(originalFilePath);

            if (mappedFile == null)
                return;

            mappedFile.NewPath = outputPath;
            RaisePropertyChange(nameof(FileMap));
        }

        public void IgnoreFile(string originalFilePath)
        {
            var mappedFile = GetFile(originalFilePath);

            if (mappedFile == null)
                return;

            mappedFile.IsIgnored = true;
            RaisePropertyChange(nameof(FileMap));
        }

        public void AcknowledgeFile(string originalFilePath)
        {
            var mappedFile = GetFile(originalFilePath);

            if (mappedFile == null)
                return;

            mappedFile.IsIgnored = false;
            RaisePropertyChange(nameof(FileMap));
        }

        public void AddAvailableProject(AvailableProjectModel model)
        {
            availableProjects.Add(model);
            RaisePropertyChange(nameof(AvailableProjectModel));
        }

        public void RemoveAvailableProject(AvailableProjectModel model)
        {
            availableProjects.Remove(model);
            RaisePropertyChange(nameof(AvailableProjectModel));
        }

        public void AddMappedFile(MappedFileModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            fileMap.Add(model);
            RaisePropertyChange(nameof(FileMap));
        }

        public void RemoveMappedFile(MappedFileModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            fileMap.Remove(model);
            RaisePropertyChange(nameof(FileMap));
        }

        private MappedFileModel GetFile(string originalFilePath)
        {
            return fileMap.FirstOrDefault(
                m => String.Equals(
                    m.OriginalPathRelativeToProject,
                    originalFilePath,
                    StringComparison.Ordinal));
        }

        private void RaisePropertyChange([CallerMemberName]string propertyName = null)
        {
            if (String.IsNullOrEmpty(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
