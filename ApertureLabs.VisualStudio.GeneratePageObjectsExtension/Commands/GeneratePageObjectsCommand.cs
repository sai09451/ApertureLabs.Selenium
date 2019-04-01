using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.CodeGeneration;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Xaml;
using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    sealed class GeneratePageObjectsCommand : BaseCommand
    {
        private delegate void ProgressReporterDelegate(string a, string b, string c, int d, int e);

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = PackageGuids.guidGeneratePageObjectsCmdSet;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly DTE dte;
        private readonly IVsSolution2 solutionService;
        private readonly IVsOutputWindow outputWindowService;
        private readonly IVsThreadedWaitDialogFactory threadedWaitDialogFactory;
        private readonly IVsMonitorSelection monitorSelectionService;
        private readonly IGeneratePageObjectsService generatePageObjectsService;
        private SynchronizePageObjectsDialog dialog;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GeneratePageObjectsCommand"/> class. Adds our command
        /// handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="dte">The DTE.</param>
        /// <param name="solutionService">The solution service.</param>
        /// <param name="outputWindowService">The output window service.</param>
        /// <param name="threadedWaitDialogFactory">The threaded wait dialog factory.</param>
        /// <param name="monitorSelectionService">The montior selection service.</param>
        /// <param name="generatePageObjectsService">The generate page objects service.</param>
        private GeneratePageObjectsCommand(AsyncPackage package,
            DTE dte,
            IVsSolution2 solutionService,
            IVsOutputWindow outputWindowService,
            IVsThreadedWaitDialogFactory threadedWaitDialogFactory,
            IVsMonitorSelection monitorSelectionService,
            IGeneratePageObjectsService generatePageObjectsService)
            : base(package)
        {
            this.package = package
                ?? throw new ArgumentNullException(nameof(package));
            this.dte = dte
                ?? throw new ArgumentNullException(nameof(dte));
            this.solutionService = solutionService
                ?? throw new ArgumentNullException(nameof(solutionService));
            this.outputWindowService = outputWindowService
                ?? throw new ArgumentNullException(nameof(outputWindowService));
            this.threadedWaitDialogFactory = threadedWaitDialogFactory
                ?? throw new ArgumentNullException(nameof(threadedWaitDialogFactory));
            this.monitorSelectionService = monitorSelectionService
                ?? throw new ArgumentNullException(nameof(monitorSelectionService));
            this.generatePageObjectsService = generatePageObjectsService
                ?? throw new ArgumentNullException(nameof(generatePageObjectsService));
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GeneratePageObjectsCommand Instance { get; private set; }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in
            // GeneratePageObjectsCommand's constructor requires the UI thread.
            await ThreadHelper.JoinableTaskFactory
                .SwitchToMainThreadAsync(package.DisposalToken);

            var dte = await package
                .GetServiceAsync(typeof(DTE))
                as DTE;
            Assumes.Present(dte);

            var solutionService = await package
                .GetServiceAsync(typeof(SVsSolution))
                as IVsSolution2;
            Assumes.Present(solutionService);

            var outputWindowService = await package
                .GetServiceAsync(typeof(SVsOutputWindow))
                as IVsOutputWindow;
            Assumes.Present(outputWindowService);

            var threadedWaitDialogFactory = await package
                .GetServiceAsync(typeof(SVsThreadedWaitDialogFactory))
                as IVsThreadedWaitDialogFactory;

            var monitorSelectionService = await package
                .GetServiceAsync(typeof(SVsShellMonitorSelection))
                as IVsMonitorSelection;

            var componentModel = await package
                .GetServiceAsync(typeof(SVsComponentModelHost))
                as IVsComponentModelHost;

            var generatePageObjectsService = await package
                .GetServiceAsync(typeof(SGeneratePageObjectsService))
                as IGeneratePageObjectsService;

            Instance = new GeneratePageObjectsCommand(package,
                dte,
                solutionService,
                outputWindowService,
                threadedWaitDialogFactory,
                monitorSelectionService,
                generatePageObjectsService);
        }

        /// <summary>
        /// Overriden by child class to setup own menu commands and bind with
        /// invocation handlers.
        /// </summary>
        protected override async Task SetupCommandsAsync()
        {
            await AddCommandAsync(
                menuGroup: PackageGuids.guidGeneratePageObjectsCmdSet,
                commandID: PackageIds.cmdidGeneratePageObjectsCommand,
                invokeHandler: Execute,
                beforeQueryHandler: BeforeQueryStatus);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var project = monitorSelectionService.GetSelectedItem() as Project;

            // Only display menu item if the project is selected.
            ((OleMenuCommand)sender).Visible = project != null;
        }

        /// <summary>
        /// This function is the callback used to execute the command when the
        /// menu item is clicked. See the constructor to see how the menu item
        /// is associated with this function using OleMenuCommandService
        /// service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(monitorSelectionService.GetSelectedItem() is Project project))
                return;

            var availableComponentTypeNames = new List<string>
            {
                "PageObject",
                "PageComponent"
            };

            var model = new SynchronizePageObjectsModel(project,
                dte,
                availableComponentTypeNames,
                solutionService);

            dialog = new SynchronizePageObjectsDialog
            {
                DataContext = model
            };

            dialog.ShowModal();
        }

        private void SynchronizePageObjects(SynchronizePageObjectsModel model)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var waitDialog = threadedWaitDialogFactory.CreateInstance();

            var task = new SyncPageObjectsTask(
                parent: this,
                model: model,
                progressReporter: (a, b, c, d, e) =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                    waitDialog.UpdateProgress(a, b, c, d, e, false, out bool pfCanceled);
                });

            // Join main thread.
            ThreadHelper.JoinableTaskFactory.RunAsyncAsVsTask(
                VsTaskRunContext.UIThreadNormalPriority,
                task.DoWorkAsync);
        }

        private void GeneratePageObject(object model)
        {
            var template = new PageComponentTemplate(model);
        }

        private void GeneratePageComponent(object model)
        {
            var template = new PageObjectTemplate(model);
        }

        private class SyncPageObjectsTask
        {
            private readonly GeneratePageObjectsCommand parent;
            private readonly SynchronizePageObjectsModel model;

            public SyncPageObjectsTask(GeneratePageObjectsCommand parent,
                SynchronizePageObjectsModel model,
                ProgressReporterDelegate progressReporter)
            {
                this.model = model
                    ?? throw new ArgumentNullException(nameof(model));

                this.parent = parent
                    ?? throw new ArgumentNullException(nameof(parent));

                Progress = progressReporter
                    ?? throw new ArgumentNullException(nameof(progressReporter));
            }

            public ProgressReporterDelegate Progress { get; }

            /// <summary>
            /// Does the work asynchronous.
            /// </summary>
            /// <param name="cancellationToken">The cancellation token.</param>
            /// <returns>
            /// Returns <see cref="VSConstants.S_OK"/> if success else returns
            /// <see cref="VSConstants.S_FALSE"/>.
            /// </returns>
            public async Task<int> DoWorkAsync(CancellationToken cancellationToken)
            {
                await ThreadHelper.JoinableTaskFactory
                    .SwitchToMainThreadAsync(cancellationToken);

                if (model.SelectedProject.IsNew)
                {
                    // I think this is the Guid for dotnet standard projects.
                    // Need to verify though.
                    var projectTypeGuid = new Guid("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
                    var projectPath = model.SelectedProject.FullPath;
                    var projectTemplatePath = String.Empty;

                    var allTemplatePaths = parent.dte.Solution
                        .ProjectItemsTemplatePath("");

                    // Check if can create project.
                    parent.solutionService.CanCreateNewProjectAtLocation(
                        fCreateNewSolution: 1,
                        pszFullProjectFilePath: model.SelectedProject.FullPath,
                        pfCanCreate: out int canCreate);

                    if (VSConstants.S_OK != canCreate)
                    {
                        // Cannot create project.
                        return VSConstants.S_FALSE;
                    }

                    // Create project.
                    parent.solutionService.CreateProject(
                        rguidProjectType: ref projectTypeGuid,
                        lpszMoniker: projectTemplatePath,
                        lpszLocation: projectPath,
                        lpszName: null,
                        grfCreateFlags: (uint)__VSCREATEPROJFLAGS.CPF_CLONEFILE,
                        iidProject: Guid.Empty,
                        ppProject: out IntPtr ppProject);
                }

                return VSConstants.S_OK;
            }
        }
    }
}
