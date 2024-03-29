﻿using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ApertureLabs.VisualStudio.SDK.Extensions.V2
{
    /// <summary>
    /// Extension methods for working with <see cref="Project"/> and
    /// <see cref="ProjectItem"/> objects.
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// Gets the project file path.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        static public string GetProjectFilePath(this IVsProject project)
        {
            string path = String.Empty;
            int hr = project.GetMkDocument((uint)VSConstants.VSITEMID.Root, out path);
            Debug.Assert(hr == VSConstants.S_OK || hr == VSConstants.E_NOTIMPL, "GetMkDocument failed for project.");

            return path;
        }

        /// <summary>
        /// Gets the project folder.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">project</exception>
        public static Uri GetProjectFolder(this Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var projectPath = new Uri(
                new FileInfo(project.FullName).Directory.FullName);

            return projectPath;
        }

        /// <summary>
        /// Gets all project items.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filesOnly">if set to <c>true</c> [files only].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public static IEnumerable<ProjectItem> GetAllProjectItems(
            this Project project,
            bool filesOnly = true)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (projectItem.IsFolder() || projectItem.Kind == null)
                {
                    if (!filesOnly)
                        yield return projectItem;

                    foreach (var item in GetAllProjectItems(projectItem, filesOnly))
                        yield return item;
                }
                else if (projectItem.IsFile())
                {
                    yield return projectItem;
                }
            }
        }

        /// <summary>
        /// Gets all project items.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <param name="filesOnly">if set to <c>true</c> [files only].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">projectItem</exception>
        public static IEnumerable<ProjectItem> GetAllProjectItems(
            this ProjectItem projectItem,
            bool filesOnly = true)
        {
            if (projectItem == null)
                throw new ArgumentNullException(nameof(projectItem));

            foreach (ProjectItem p in projectItem.ProjectItems)
            {
                if (p.IsFolder())
                {
                    if (!filesOnly)
                        yield return p;

                    foreach (var item in GetAllProjectItems(p, filesOnly))
                        yield return item;
                }
                else if (p.IsFile())
                {
                    yield return p;
                }
            }
        }

        /// <summary>
        /// Determines whether this instance is folder.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is folder; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">projectItem</exception>
        public static bool IsFolder(this ProjectItem projectItem)
        {
            if (projectItem == null)
                throw new ArgumentNullException(nameof(projectItem));

            switch (projectItem.Kind)
            {
                case VSConstants.ItemTypeGuid.PhysicalFolder_string:
                case VSConstants.ItemTypeGuid.VirtualFolder_string:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether this instance is file.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <returns>
        ///   <c>true</c> if the specified project item is file; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">projectItem</exception>
        public static bool IsFile(this ProjectItem projectItem)
        {
            if (projectItem == null)
                throw new ArgumentNullException(nameof(projectItem));

            switch (projectItem.Kind)
            {
                case VSConstants.ItemTypeGuid.PhysicalFile_string:
                    return true;
                default:
                    return false;
            }
        }
    }
}
