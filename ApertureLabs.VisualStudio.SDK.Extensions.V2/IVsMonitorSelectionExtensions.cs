using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ApertureLabs.VisualStudio.SDK.Extensions.V2
{
    /// <summary>
    /// Contains extension methods for <see cref="IVsMonitorSelection"/>
    /// </summary>
    public static class IVsMonitorSelectionExtensions
    {
        /// <summary>
        /// Gets the selected item of the <see cref="IVsMonitorSelection"/>.
        /// </summary>
        /// <param name="monitorSelection">The monitor selection.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">monitorSelection</exception>
        public static object GetSelectedItem(this IVsMonitorSelection monitorSelection)
        {
            if (monitorSelection == null)
                throw new ArgumentNullException(nameof(monitorSelection));

            var selectedObject = default(object);

            try
            {
                monitorSelection.GetCurrentSelection(
                    out IntPtr hierarchyPointer,
                    out uint itemId,
                    out IVsMultiItemSelect multiItemSelect,
                    out IntPtr selectionContianerPointer);

                var selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                    hierarchyPointer,
                    typeof(IVsHierarchy))
                    as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    ErrorHandler.ThrowOnFailure(selectedHierarchy.GetProperty(
                        itemId,
                        (int)__VSHPROPID.VSHPROPID_ExtObject,
                        out selectedObject));
                }

                Marshal.Release(hierarchyPointer);
                Marshal.Release(selectionContianerPointer);
            }
            catch(Exception exception)
            {
                VsShellUtilities.LogError(exception.Source, exception.ToString());
            }

            return selectedObject;
        }
    }
}
