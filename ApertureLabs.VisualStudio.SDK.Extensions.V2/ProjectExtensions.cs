using EnvDTE;
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
        /// <param name="includeFolders">if set to <c>true</c> [include folders].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">project</exception>
        public static IEnumerable<ProjectItem> GetAllProjectItems(
            this Project project,
            bool includeFolders = false)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (projectItem.IsFolder())
                {
                    if (includeFolders)
                        yield return projectItem;

                    foreach (var item in GetAllProjectItems(projectItem))
                        yield return item;
                }
                else
                {
                    yield return projectItem;
                }
            }
        }

        /// <summary>
        /// Gets all project items.
        /// </summary>
        /// <param name="projectItem">The project item.</param>
        /// <param name="includeFolders">if set to <c>true</c> [include folders].</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">projectItem</exception>
        public static IEnumerable<ProjectItem> GetAllProjectItems(
            this ProjectItem projectItem,
            bool includeFolders = false)
        {
            if (projectItem == null)
                throw new ArgumentNullException(nameof(projectItem));

            foreach (ProjectItem p in projectItem.ProjectItems)
            {
                if (p.IsFolder())
                {
                    if (includeFolders)
                        yield return p;

                    foreach (var item in GetAllProjectItems(projectItem))
                        yield return item;
                }
                else
                {
                    yield return projectItem;
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
    }
}
