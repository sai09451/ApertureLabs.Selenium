using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services
{
    public class GeneratePageObjectsService : SGeneratePageObjectsService,
        IGeneratePageObjectsService
    {
        #region Fields

        [Import]
        private IVsPackageInstallerServices packageInstallerService;

        [Import]
        private IVsPackageInstaller packageInstaller;

        private DTE dte;
        private IVsSolution2 solutionService;
        private IVsOutputWindow outputWindowService;
        private IVsThreadedWaitDialogFactory threadedWaitDialogFactory;
        private IVsMonitorSelection monitorSelectionService;

        #endregion

        #region Constructor

        public GeneratePageObjectsService()
        { }

        #endregion

        #region Methods

        public async Task InitializeServiceAsync(CancellationToken cancellationToken)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            dte = ServiceProvider.GlobalProvider
                .GetService(typeof(DTE)) as DTE;
            Assumes.Present(dte);

            solutionService = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsSolution)) as IVsSolution2;
            Assumes.Present(solutionService);

            outputWindowService = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            Assumes.Present(outputWindowService);

            threadedWaitDialogFactory = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsThreadedWaitDialogFactory)) as IVsThreadedWaitDialogFactory;
            Assumes.Present(threadedWaitDialogFactory);

            monitorSelectionService = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            Assumes.Present(monitorSelectionService);
        }

        //public SynchronizePageObjectsModel GetSyncModel(Project project)
        //{
        //    ThreadHelper.ThrowIfNotOnUIThread();

        //    if (project == null)
        //        throw new ArgumentNullException(nameof(project));

        //    var model = new SynchronizePageObjectsModel(project,
        //        dte,
        //        availableComponentTypeNames,
        //        solutionService);

        //    // Get the solution folder.
        //    var solutionDir = new FileInfo(dte.Solution.FullName)
        //        .Directory
        //        .FullName;

        //    var defaultProjectName = $"{project.Name}.PageObjects";
        //    model.DefaultNamespace = defaultProjectName;
        //    model.OriginalProjectName = project.Name;
        //    var newProject = model.AvailableProjects[0];
        //    model.AvailableComponentTypeNames = AvailableComponentTypeNames();

        //    newProject.FullPath = Path.Combine(
        //        solutionDir,
        //        defaultProjectName);

        //    var projects = solutionService.GetProjects();

        //    foreach (var p in projects)
        //    {
        //        model.AddAvailableProject(new AvailableProjectModel
        //        {
        //            DisplayName = p.Name,
        //            FullPath = p.FullName,
        //            IsNew = false,
        //            UniqueName = p.UniqueName,
        //        });
        //    }

        //    // Check if a default project already exists.
        //    model.SelectedProjectIndex = model
        //        .AvailableProjects
        //        .Select((m, i) => new { Model = m, Index = i })
        //        .FirstOrDefault(m => m.Model.DisplayName == defaultProjectName)
        //        ?.Index
        //        ?? 0;

        //    // Retrieve the project folder path.
        //    var projectPath = new Uri(
        //        new FileInfo(project.FullName).Directory.FullName);

        //    var selectedProjectPath = model.SelectedProject.FullPath;

        //    // Now locate all razor files in the selected project.
        //    foreach (var item in project.GetAllProjectItems())
        //    {
        //        var name = item.Name;

        //        var extension = Path.GetExtension(item.Name);
        //        var isRazorFile = extension.Equals(
        //            ".cshtml",
        //            StringComparison.Ordinal);

        //        if (!isRazorFile)
        //            continue;

        //        var fullPath = new Uri(item.Properties
        //            ?.Item("FullPath")
        //            ?.Value
        //            ?.ToString());

        //        var relativePath = projectPath
        //            .MakeRelativeUri(fullPath)
        //            .ToString();

        //        var seperator = Path.DirectorySeparatorChar;

        //        if (!relativePath.Contains(seperator))
        //            seperator = Path.AltDirectorySeparatorChar;

        //        // This is to remove the 'base' dir of the relative path.
        //        relativePath = relativePath.Remove(
        //            0,
        //            relativePath.IndexOf(seperator) + 1);

        //        // Calling Path.GetFullPath will normalize the path.
        //        var newFullPath = Path.GetFullPath(
        //            Path.Combine(
        //                selectedProjectPath,
        //                relativePath));

        //        var mappedFile = new MappedFileModel
        //        {
        //            IsIgnored = false,
        //            IsNewFile = false,
        //            SelectedComponentTypeNameIndex = 0,
        //            NewPath = newFullPath,
        //            OriginalPathRelativeToProject = relativePath,
        //            FileName = item.Name,
        //            ProjectItemReference = item,
        //            AvailableComponentTypeNames = model.AvailableComponentTypeNames
        //        };

        //        model.AddMappedFile(mappedFile);
        //    }

        //    return model;
        //}

        public string DetermineComponentType(ProjectItem projectItem)
        {
            if (projectItem == null)
                throw new ArgumentNullException(nameof(projectItem));

            throw new NotImplementedException();
        }

        public void InstallApertureLibrariesInProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var installedPackages = packageInstallerService
                .GetInstalledPackages(project);

            // Check if package is already installed.
            foreach (var package in installedPackages)
            {
                if (package.Title.Equals("ApertureLabs.Selenium", StringComparison.Ordinal))
                    return;
            }

            packageInstaller.InstallPackage(
                source: null,
                project: project,
                packageId: "ApertureLabs.Selenium",
                version: default(Version),
                ignoreDependencies: false);
        }

        private IReadOnlyList<string> AvailableComponentTypeNames()
        {
            return new List<string>()
            {
                "PageObject",
                "IPageObject",
                "PageComponent",
                "IPageComponent",
                "StaticPageObject",
                "BasePageObject",
                "ParameterPageObject"
            };
        }

        private void ImplementedInterface(Type interfaceType,
            ProjectItem razorProjectItem,
            ProjectItem generatedItem)
        {

        }

        #endregion
    }
}
