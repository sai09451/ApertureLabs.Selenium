using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ApertureLabs.GeneratePageObjectsExtension.Helpers
{
    public static class ProjectHelpers
    {
        /// <summary>
        /// Returns either a Project or ProjectItem. Returns null if Solution
        /// is selected.
        /// </summary>
        /// <returns></returns>
        public static object GetSelectedItem()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            IntPtr hierarchyPointer, selectionContainerPointer;
            object selectedObject = null;
            IVsMultiItemSelect multiItemSelect;
            uint itemId;

            var monitorSelection = Package
                .GetGlobalService(typeof(SVsShellMonitorSelection))
                as IVsMonitorSelection;

            try
            {
                monitorSelection.GetCurrentSelection(out hierarchyPointer,
                    out itemId,
                    out multiItemSelect,
                    out selectionContainerPointer);

                var selectedHierarchy = Marshal.GetTypedObjectForIUnknown(
                    hierarchyPointer,
                    typeof(IVsHierarchy))
                    as IVsHierarchy;

                if (selectedHierarchy != null)
                {
                    ErrorHandler.ThrowOnFailure(
                        selectedHierarchy.GetProperty(
                            itemId,
                            (int)__VSHPROPID.VSHPROPID_ExtObject,
                            out selectedObject));
                }

                Marshal.Release(hierarchyPointer);
                Marshal.Release(selectionContainerPointer);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

            return selectedObject;
        }

        public static IEnumerable<Project> GetAllProjectsInSolution(
            IServiceProvider serviceProvider,
            IVsSolution solution = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            solution = solution
                ?? (IVsSolution)serviceProvider.GetService(typeof(IVsSolution));

            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                Project project = GetDTEProject(hier);
                if (project != null)
                    yield return project;
            }
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return GetProjectsInSolution(
                solution,
                __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        public static IEnumerable<IVsHierarchy> GetProjectsInSolution(IVsSolution solution, __VSENUMPROJFLAGS flags)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (solution == null)
                yield break;

            Guid guid = Guid.Empty;
            solution.GetProjectEnum(
                (uint)flags,
                ref guid,
                out IEnumHierarchies enumHierarchies);

            if (enumHierarchies == null)
                yield break;

            IVsHierarchy[] hierarchy = new IVsHierarchy[1];
            uint fetched;
            while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
            {
                if (hierarchy.Length > 0 && hierarchy[0] != null)
                    yield return hierarchy[0];
            }
        }

        public static Project GetDTEProject(IVsHierarchy hierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (hierarchy == null)
                throw new ArgumentNullException(nameof(hierarchy));

            hierarchy.GetProperty(
                VSConstants.VSITEMID_ROOT,
                (int)__VSHPROPID.VSHPROPID_ExtObject,
                out object obj);

            return obj as Project;
        }

        public static void CheckFileOutOfSourceControl(
            IServiceProvider serviceProvider,
            string file)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = (DTE)serviceProvider.GetService(typeof(DTE));
            Assumes.Present(dte);

            if (!File.Exists(file) || dte.Solution.FindProjectItem(file) == null)
                return;

            if (dte.SourceControl.IsItemUnderSCC(file) && !dte.SourceControl.IsItemCheckedOut(file))
                dte.SourceControl.CheckOutItem(file);

            FileInfo info = new FileInfo(file)
            {
                IsReadOnly = false
            };
        }

        public static ProjectItem AddFileToProject(
            IServiceProvider serviceProvider,
            Project project,
            string file,
            string itemType = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!File.Exists(file))
                return null;

            var dte = (DTE)serviceProvider.GetService(typeof(DTE));
            Assumes.Present(dte);

            var item = dte.Solution.FindProjectItem(file);

            if (item == null)
            {
                item = project.ProjectItems.AddFromFile(file);
                item.SetItemType(itemType);
            }

            return item;
        }

        public static void SetItemType(this ProjectItem item, string itemType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (item == null || item.ContainingProject == null)
                    return;

                item.Properties.Item("ItemType").Value = itemType;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        public static string GetRootFolder(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null || String.IsNullOrEmpty(project.FullName))
                return null;

            string fullPath;

            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (String.IsNullOrEmpty(fullPath))
            {
                return File.Exists(project.FullName)
                    ? Path.GetDirectoryName(project.FullName)
                    : null;
            }
            else if (Directory.Exists(fullPath))
            {
                return fullPath;
            }
            else
            {
                return File.Exists(fullPath)
                    ? Path.GetDirectoryName(fullPath)
                    : null;
            }
        }

        /// <summary>
        /// Gets the paths to all files included in the selection, including
        /// files within selected folders.
        /// </summary>
        public static IEnumerable<string> GetSelectedFilePaths(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            return GetSelectedItemPaths(serviceProvider)
                .SelectMany(p => Directory.Exists(p)
                    ? Directory.EnumerateFiles(p, "*", SearchOption.AllDirectories)
                    : new[] { p });
        }


        /// <summary>
        /// Gets the full paths to the currently selected item(s) in the
        /// Solution Explorer.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public static IEnumerable<string> GetSelectedItemPaths(IServiceProvider serviceProvider)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var dte = serviceProvider.GetService(typeof(DTE)) as DTE2;
            Assumes.Present(dte);

            var items = (Array)dte.ToolWindows.SolutionExplorer.SelectedItems;
            foreach (UIHierarchyItem selItem in items)
            {
                if (selItem.Object is ProjectItem item)
                {
                    if (item.Properties != null)
                    {
                        yield return item.Properties
                            .Item("FullPath")
                            .Value
                            .ToString();
                    }
                }
            }
        }

    }
}
