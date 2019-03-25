using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace ApertureLabs.GeneratePageObjectsExtension.Helpers
{
    /// <summary>
    /// Used for debugging purposes.
    /// </summary>
    /// <remarks>
    /// See https://github.com/madskristensen/ExtensibilityTools/blob/84db9c4be49f1a77b1b6f049df90813f7f67cc9b/src/Shared/Helpers/Logger.cs.
    /// </remarks>
    internal static class Logger
    {
        private static string name;
        private static IVsOutputWindowPane pane;
        private static IVsOutputWindow output;

        public static async Task InitializeAsync(AsyncPackage provider, string name)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            Logger.name = name;

            output = await provider
                .GetServiceAsync(typeof(SVsOutputWindow))
                as IVsOutputWindow;
            Assumes.Present(output);
        }


        public static void Log(object message)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (EnsurePane())
                {
                    pane.OutputString(DateTime.Now.ToString() + ": " + message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private static bool EnsurePane()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (pane == null)
            {
                Guid guid = Guid.NewGuid();
                output.CreatePane(ref guid, name, 1, 1);
                output.GetPane(ref guid, out pane);
            }

            return pane != null;
        }

    }
}
