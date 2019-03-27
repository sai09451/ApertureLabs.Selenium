using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ApertureLabs.GeneratePageObjectsExtension.Models
{
    public class SynchronizePageObjectsModel : INotifyPropertyChanged
    {
        #region Fields

        private readonly List<AvailableProjectModel> availableProjects;
        private readonly List<MappedFileModel> fileMap;

        private string pageObjectLibraryName;
        private bool useAreas;
        private int selectedProjectIndex;
        private string originalProjectName;
        private string defaultNamespace;
        private string pathToNewProject;

        #endregion

        #region Constructor

        public SynchronizePageObjectsModel()
        {
            availableProjects = new List<AvailableProjectModel>
            {
                new AvailableProjectModel
                {
                    IsNew = true,
                    DisplayName = "New",
                    UniqueName = String.Empty
                }
            };
            fileMap = new List<MappedFileModel>();
            useAreas = true;
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

                selectedProjectIndex = value;
                RaisePropertyChange();
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

        public string PathToNewProject
        {
            get => pathToNewProject;
            set
            {
                pathToNewProject = value;
                RaisePropertyChange();
            }
        }

        #endregion

        #region Methods

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
                    m.OriginalPath,
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
