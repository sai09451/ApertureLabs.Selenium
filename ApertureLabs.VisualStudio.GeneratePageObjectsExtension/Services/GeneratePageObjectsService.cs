using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
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

        private const string PACKAGE_APT_SELENIUM = "ApertureLabs.Selenium";
        private const string PACKAGE_APT_SELENIUM_CODE_GENERATION = "ApertureLabs.Selenium.CodeGeneration";

        private IVsPackageInstallerServices packageInstallerService;
        private IVsPackageInstaller packageInstaller;
        private IVsPackageRestorer packageRestorer;
        private EnvDTE80.DTE2 dte;
        private IVsSolution2 solutionService;
        private IVsOutputWindow outputWindowService;
        private IVsThreadedWaitDialogFactory threadedWaitDialogFactory;
        private IVsMonitorSelection monitorSelectionService;
        private IVsUIShell shellService;
        private IVsPreviewChangesService previewChangesService;

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
                .GetService(typeof(SDTE)) as EnvDTE80.DTE2;
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

            shellService = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsUIShell)) as IVsUIShell;
            Assumes.Present(shellService);

            previewChangesService = ServiceProvider.GlobalProvider
                .GetService(typeof(SVsPreviewChangesService))
                as IVsPreviewChangesService;
            Assumes.Present(previewChangesService);

            var componentModel = ServiceProvider.GlobalProvider
                .GetService(typeof(SComponentModel))
                as IComponentModel;
            Assumes.Present(componentModel);

            packageInstallerService = componentModel
                .GetService<IVsPackageInstallerServices>();
            Assumes.Present(packageInstallerService);

            packageInstaller = componentModel
                .GetService<IVsPackageInstaller>();
            Assumes.Present(packageInstaller);
        }

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

        public async Task GeneratePageObjectsAsync(SynchronizePageObjectsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var cancellationToken = CancellationToken.None;
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            if (model.SelectedProject.IsNew)
            {
                //var projectType = new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
                var projectType = new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
                var idProject = Guid.NewGuid();
                var projectTemplatePath = dte.Solution.ProjectItemsTemplatePath("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");

                solutionService.CanCreateNewProjectAtLocation(
                    fCreateNewSolution: Convert.ToInt32(false),
                    pszFullProjectFilePath: model.SelectedProject.FullPath,
                    pfCanCreate: out int canCreate);

                if (!Convert.ToBoolean(canCreate))
                {
                    // Cannot create the project.
                    VsShellUtilities.ShowMessageBox(
                        ServiceProvider.GlobalProvider,
                        "Project already exists with that name.",
                        "Failed to create the new project",
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }

                // Create project.
                var projectCreateResult = solutionService.CreateProject(
                    rguidProjectType: ref projectType,
                    lpszMoniker: projectTemplatePath,
                    lpszLocation: model.SelectedProject.PathToProjectFolder,
                    lpszName: model.SelectedProject.Name,
                    grfCreateFlags: (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE,
                    iidProject: ref idProject,
                    ppProject: out IntPtr projectPtr);

                if (ErrorHandler.Failed(projectCreateResult))
                {
                    if (ErrorHandler.Failed(shellService.GetErrorInfo(out var errorText)))
                        errorText = "Failed to create the new project.";

                    VsShellUtilities.ShowMessageBox(
                        ServiceProvider.GlobalProvider,
                        errorText,
                        String.Empty,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }
            }

            // Retrieve the project.
            var project = solutionService
                    .GetProjects()
                    .FirstOrDefault(
                        p => p.Name.Equals(
                            model.SelectedProject.Name,
                            StringComparison.OrdinalIgnoreCase));

            var vsProject = solutionService.GetProjectByFileName(project.FileName);

            if (project == null)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider.GlobalProvider,
                    "Failed to locate the project.",
                    String.Empty,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return;
            }

            // Verify the project has the necessary files installed.
            var isCodeGenPackageInstalled = packageInstallerService
                .IsPackageInstalled(
                    project,
                    PACKAGE_APT_SELENIUM_CODE_GENERATION);

            if (!AreCodeGenerationPackagesInstalled(project))
            {
                try
                {
                    packageInstaller.InstallPackage(
                        source: null,
                        project: project,
                        packageId: PACKAGE_APT_SELENIUM_CODE_GENERATION,
                        version: default(string),
                        ignoreDependencies: false);
                }
                catch (Exception e)
                {
                    VsShellUtilities.ShowMessageBox(
                        ServiceProvider.GlobalProvider,
                        e.ToString(),
                        String.Empty,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                    return;
                }
            }

            // Retrieve all CodeGenerators.
            var codeGenerators = GetAllCodeGenerators(project);

            if (!codeGenerators.Any())
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider.GlobalProvider,
                    "Failed to locate any code generators in the project.",
                    String.Empty,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return;
            }

            // Create and display a wait dialog.
            var dialog = threadedWaitDialogFactory.StartWaitDialog(
                waitCaption: "Starting",
                initialProgress: new ThreadedWaitDialogProgressData(
                    waitMessage: "Starting",
                    progressText: String.Empty,
                    statusBarText: String.Empty,
                    isCancelable: true,
                    currentStep: 0,
                    totalSteps: model.FileMap.Count),
                delayToShowDialog: TimeSpan.Zero);

            using (dialog)
            {
                // Iterate over all file maps in the model.
                for (var currentStep = 0; currentStep < model.FileMap.Count; currentStep++)
                {
                    // Exit loop if canceled.
                    if (dialog.UserCancellationToken.IsCancellationRequested)
                        break;

                    var fileMap = model.FileMap[currentStep];
                    //var projectItem = default(ProjectItem);

                    // Check if file exists.
                    if (fileMap.IsNewFile)
                    {
                        // Create file.
                        var fileInfo = new FileInfo(fileMap.NewPath);
                        Directory.CreateDirectory(fileInfo.Directory.FullName);
                        File.Create(fileMap.NewPath);

                        // Add file to the project.
                        if (project.Kind != "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
                            && vsProject != null)
                        {
                            var result = new VSADDRESULT[1];

                            vsProject.AddItem(
                                itemidLoc: (uint)VSConstants.VSITEMID.Root,
                                dwAddItemOperation: VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE,
                                pszItemName: fileMap.NewPath,
                                cFilesToOpen: 0,
                                rgpszFilesToOpen: new[] { fileMap.NewPath },
                                hwndDlgOwner: IntPtr.Zero,
                                pResult: result);

                            if (result[0].Equals(VSADDRESULT.ADDRESULT_Failure))
                            {
                                VsShellUtilities.ShowMessageBox(
                                    ServiceProvider.GlobalProvider,
                                    "Failed to add the item to the project.",
                                    String.Empty,
                                    OLEMSGICON.OLEMSGICON_CRITICAL,
                                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                                return;
                            }
                        }
                    }

                    // TODO: Generate code for file.

                    // Update wait dialog.
                    dialog.Progress.Report(new ThreadedWaitDialogProgressData(
                        waitMessage: String.Empty,
                        progressText: String.Empty,
                        statusBarText: String.Empty,
                        isCancelable: true,
                        currentStep: currentStep,
                        totalSteps: model.FileMap.Count));
                }
            }
        }

        private bool AreCodeGenerationPackagesInstalled(Project project)
        {
            var isCodeGenPackageInstalled = packageInstallerService
                .IsPackageInstalled(
                    project,
                    PACKAGE_APT_SELENIUM_CODE_GENERATION);

            return isCodeGenPackageInstalled;
        }

        private IReadOnlyList<string> AvailableComponentTypeNames()
        {
            // TODO: Retrieve component type names from the project.
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

        private IEnumerable<object> GetAllCodeGenerators(Project project)
        {
            // TODO: Implemented method.
            return Enumerable.Empty<object>();
        }

        private void ImplementedInterface(Type interfaceType,
            ProjectItem razorProjectItem,
            ProjectItem generatedItem)
        {

        }

        #endregion
    }
}
