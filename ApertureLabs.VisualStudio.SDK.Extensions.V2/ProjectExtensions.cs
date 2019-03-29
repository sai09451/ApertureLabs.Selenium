using EnvDTE;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;

namespace ApertureLabs.VisualStudio.SDK.Extensions.V2
{
    /// <summary>
    /// Extension methods for working with <see cref="Project"/> and
    /// <see cref="ProjectItem"/> objects.
    /// </summary>
    public static class ProjectExtensions
    {
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
