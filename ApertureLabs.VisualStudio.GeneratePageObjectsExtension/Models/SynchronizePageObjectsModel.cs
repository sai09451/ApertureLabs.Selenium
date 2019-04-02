using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models
{
    public class SynchronizePageObjectsModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly List<AvailableProjectModel> availableProjects;
        private readonly List<MappedFileModel> fileMap;

        private List<string> availableComponentTypeNames;
        private string pageObjectLibraryName;
        private int selectedProjectIndex;
        private string originalProjectName;
        private string defaultNamespace;

        #endregion

        #region Constructor

        public SynchronizePageObjectsModel(
            Project project,
            IReadOnlyList<string> availableComponentTypeNames,
            DTE dte,
            IVsSolution2 solutionService)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null)
                throw new ArgumentNullException(nameof(project));
            else if (availableComponentTypeNames == null)
                throw new ArgumentNullException(nameof(availableComponentTypeNames));
            else if (dte == null)
                throw new ArgumentNullException(nameof(dte));
            else if (solutionService == null)
                throw new ArgumentNullException(nameof(solutionService));

            fileMap = new List<MappedFileModel>();
            AvailableComponentTypeNames = availableComponentTypeNames;
            var defaultProjectName = $"{project.Name}.PageObjects";
            DefaultNamespace = defaultProjectName;
            OriginalProjectName = project.Name;

            // Get the solution folder.
            var solutionDir = new FileInfo(dte.Solution.FullName)
                .Directory
                .FullName;

            availableProjects = new List<AvailableProjectModel>
            {
                new AvailableProjectModel
                {
                    IsNew = true,
                    DisplayName = "New",
                    Name = defaultProjectName,
                    PathToProjectFolder = Path.Combine(
                        solutionDir,
                        defaultProjectName),
                    FullPath = Path.Combine(
                        solutionDir,
                        defaultProjectName,
                        defaultProjectName + ".csproj"),
                    UniqueName = String.Empty
                }
            };

            var newProject = AvailableProjects[0];
            var projects = solutionService.GetProjects();

            foreach (var p in projects)
            {
                if (String.IsNullOrEmpty(p?.FullName))
                    continue;

                AddAvailableProject(new AvailableProjectModel
                {
                    DisplayName = p.Name,
                    FullPath = p.FileName,
                    IsNew = false,
                    Name = p.Name,
                    PathToProjectFolder = new FileInfo(p.FullName).Directory.FullName,
                    UniqueName = p.UniqueName,
                });
            }

            // Check if a default project already exists.
            SelectedProjectIndex = AvailableProjects
                .Select((m, i) => new { Model = m, Index = i })
                .FirstOrDefault(m => m.Model.DisplayName == defaultProjectName)
                ?.Index
                ?? 0;

            // Now locate all razor files in the selected project.
            foreach (var item in project.GetAllProjectItems())
            {
                // Ignore if not a razor file.
                var extension = Path.GetExtension(item.Name);
                var isRazorFile = extension.Equals(
                    ".cshtml",
                    StringComparison.Ordinal);

                if (!isRazorFile)
                    continue;

                var mappedFile = new MappedFileModel(project,
                    item,
                    availableComponentTypeNames,
                    SelectedProject.PathToProjectFolder);

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
                    fileMap.UpdateProjectPath(selectedProj);

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
                    m.RelativePathToFileFromProject,
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
