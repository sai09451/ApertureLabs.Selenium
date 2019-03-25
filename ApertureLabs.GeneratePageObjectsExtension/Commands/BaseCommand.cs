using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using IAsyncServiceProvider = Microsoft.VisualStudio.Shell.IAsyncServiceProvider;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.GeneratePageObjectsExtension.Commands
{
    /// <summary>
    /// Basic class to wrap code about executed menu command.
    /// </summary>
    /// <remarks>
    /// See https://github.com/madskristensen/ExtensibilityTools/blob/84db9c4be49f1a77b1b6f049df90813f7f67cc9b/src/Shared/Commands/BaseCommand.cs
    /// </remarks>
    abstract class BaseCommand
    {
        protected BaseCommand(IAsyncServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));

            SetupCommandsAsync();
        }

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        /// <value>
        /// The service provider.
        /// </value>
        protected IAsyncServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Setups new menu command with handlers.
        /// </summary>
        protected async Task<OleMenuCommand> AddCommandAsync(Guid menuGroup,
            int commandID,
            EventHandler invokeHandler,
            EventHandler beforeQueryHandler)
        {
            if (invokeHandler == null)
                throw new ArgumentNullException("invokeHandler", "Missing action to perform");

            OleMenuCommandService commandService =
                await GetServiceAsync<OleMenuCommandService, IMenuCommandService>();

            if (commandService != null)
            {
                OleMenuCommand addCustomToolItem = new OleMenuCommand(
                    invokeHandler, new CommandID(
                        menuGroup,
                        commandID));

                if (beforeQueryHandler != null)
                {
                    addCustomToolItem.BeforeQueryStatus += beforeQueryHandler;
                }

                commandService.AddCommand(addCustomToolItem);
            }

            return null;
        }

        /// <summary>
        /// Gets the specific service.
        /// </summary>
        protected async Task<T> GetServiceAsync<T, S>() where T : class
        {
            return await ServiceProvider.GetServiceAsync(typeof(S)) as T;
        }

        /// <summary>
        /// Gets the specific service.
        /// </summary>
        protected async Task<T> GetServiceAsync<T>() where T : class
        {
            return await ServiceProvider.GetServiceAsync(typeof(T)) as T;
        }

        /// <summary>
        /// Overriden by child class to setup own menu commands and bind with invocation handlers.
        /// </summary>
        protected abstract Task SetupCommandsAsync();
    }
}
