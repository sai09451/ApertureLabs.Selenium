using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.GeneratePageObjectsExtension.CodeGeneration;
using ApertureLabs.GeneratePageObjectsExtension.Helpers;
using ApertureLabs.GeneratePageObjectsExtension.Models;
using ApertureLabs.VisualStudio.SDK.Extensions;
using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.GeneratePageObjectsExtension.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    sealed class GeneratePageObjectsCommand : BaseCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("28086d5f-a1ae-49c8-b3b9-e854c4ee13fa");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        private readonly DTE dte;
        private readonly IVsSolution2 solutionService;
        private readonly IVsOutputWindow outputWindowService;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GeneratePageObjectsCommand"/> class. Adds our command
        /// handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GeneratePageObjectsCommand(AsyncPackage package,
            DTE dte,
            IVsSolution2 solutionService,
            IVsOutputWindow outputWindowService)
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

            Instance = new GeneratePageObjectsCommand(package,
                dte,
                solutionService,
                outputWindowService);
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

            var project = ProjectHelpers.GetSelectedItem() as Project;

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

            if (!(ProjectHelpers.GetSelectedItem() is Project project))
                return;

            var model = new SynchronizePageObjectsModel();
            var pathToProject = new FileInfo(project.FullName).Directory.FullName;
            var defaultProjectName = $"{project.Name}.PageObjects";
            model.PathToNewProject = Path.Combine(
                pathToProject,
                defaultProjectName);
            var projects = solutionService.GetProjects();

            foreach (var p in projects)
            {
                model.AddAvailableProject(new AvailableProjectModel
                {
                    DisplayName = p.Name,
                    UniqueName = p.UniqueName,
                    IsNew = false
                });
            }

            // Check if a default project already exists.
            model.SelectedProjectIndex = model
                .AvailableProjects
                .Select((m, i) => new { Model = m, Index = i })
                .FirstOrDefault(m => m.Model.DisplayName == defaultProjectName)
                ?.Index
                ?? 0;

            // Now locate all razor files in the selected project.
            foreach (var item in project.GetAllProjectItems())
            {
                var isRazorFile = Path.GetExtension(item.Name).Equals(
                    "cshtml",
                    StringComparison.Ordinal);

                if (!isRazorFile)
                    continue;

                var fullPath = item.Properties
                    ?.Item("FullPath")
                    ?.Value
                    ?.ToString();

                var mappedFile = new MappedFileModel
                {
                    IsIgnored = false,
                    IsNewFile = false,
                    IsPageComponent = false,
                    NewPath = String.Empty,
                    OriginalPath = fullPath,
                    ProjectItemReference = item
                };

                model.AddMappedFile(mappedFile);
            }

            var modalWindow = new SynchronizePageObjectsDialog
            {
                DataContext = model
            };

            modalWindow.ShowModal();

            //string message = String.Format(
            //    CultureInfo.CurrentCulture,
            //    "Inside {0}.MenuItemCallback()",
            //    GetType().FullName);

            //string title = "GeneratePageObjectsCommand";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    package,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void SynchronizePageObjects(SynchronizePageObjectsModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var task = new SyncPageObjectsTask(model);

            // Join main thread.
            ThreadHelper.JoinableTaskFactory.RunAsyncAsVsTask(
                    VsTaskRunContext.UIThreadNormalPriority,
                    SyncPageObjectsTask)
                .ContinueWith(0, task);
        }

        private Task<object> SyncPageObjectsTask(CancellationToken cancellationToken)
        {
            return Task.CompletedTask.ContinueWith(t => (object)43);
        }

        private void GeneratePageObject(object model)
        {
            var template = new PageComponentTemplate(model);
        }

        private void GeneratePageComponent(object model)
        {
            var template = new PageObjectTemplate(model);
        }

        private class SyncPageObjectsTask : IVsTaskBody
        {
            private readonly SynchronizePageObjectsModel model;

            public SyncPageObjectsTask(SynchronizePageObjectsModel model)
            {
                this.model = model;
            }

            public void DoWork(IVsTask pTask,
                uint dwCount,
                IVsTask[] pParentTasks,
                out object pResult)
            {
                throw new NotImplementedException();
            }
        }
    }
}
