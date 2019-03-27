using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.GeneratePageObjectsExtension.Helpers;
using EnvDTE;
using Microsoft;
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

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="GeneratePageObjectsCommand"/> class. Adds our command
        /// handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private GeneratePageObjectsCommand(AsyncPackage package)
            : base(package)
        {
            this.package = package
                ?? throw new ArgumentNullException(nameof(package));
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static GeneratePageObjectsCommand Instance
        {
            get;
            private set;
        }

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

            //var commandService = await package
            //    .GetServiceAsync(typeof(IMenuCommandService))
            //    as OleMenuCommandService;
            //Assumes.Present(commandService);

            //var dteService = await package
            //    .GetServiceAsync(typeof(DTE))
            //    as DTE;
            //Assumes.Present(dteService);

            //var solutionService = await package
            //    .GetServiceAsync(typeof(SVsSolution))
            //    as IVsSolution2;
            //Assumes.Present(solutionService);

            //var outputPaneService = await package
            //    .GetServiceAsync(typeof(SVsOutputWindow))
            //    as IVsOutputWindow;
            //Assumes.Present(outputPaneService);

            //var shellService = await package
            //    .GetServiceAsync(typeof(SVsUIShell))
            //    as IVsUIShell;
            //Assumes.Present(shellService);

            Instance = new GeneratePageObjectsCommand(package);
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

            string message = String.Format(
                CultureInfo.CurrentCulture,
                "Inside {0}.MenuItemCallback()",
                GetType().FullName);

            string title = "GeneratePageObjectsCommand";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
