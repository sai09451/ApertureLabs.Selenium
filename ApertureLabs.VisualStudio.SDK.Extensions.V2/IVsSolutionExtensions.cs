using ApertureLabs.VisualStudio.SDK.Extensions.V2;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.VisualStudio.SDK.Extensions
{
    /// <summary>
    /// Contains extensions for working with <see cref="IVsSolution"/>.
    /// </summary>
    public static class IVsSolutionExtensions
    {
        public static IEnumerable<IVsProject> GetProjectsOfCurrentSolution(
            this IVsSolution solutionService)
        {
            if (solutionService == null)
                throw new ArgumentNullException(nameof(solutionService));

            IEnumHierarchies enumerator = null;
            Guid guid = Guid.Empty;
            solutionService.GetProjectEnum(
                (uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION,
                ref guid,
                out enumerator);
            IVsHierarchy[] hierarchy = new IVsHierarchy[1] { null };
            uint fetched = 0;

            for (enumerator.Reset();
                enumerator.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1;
                /*nothing*/)
            {
                yield return (IVsProject)hierarchy[0];
            }
        }

        static public IVsProject GetProjectByFileName(
            this IVsSolution solutionService,
            string projectFile)
        {
            return GetProjectsOfCurrentSolution(solutionService).FirstOrDefault(
                p => String.Compare(
                    projectFile,
                    p.GetProjectFilePath(),
                    StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// Gets the projects of the solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns></returns>
        public static IEnumerable<EnvDTE.Project> GetProjects(this IVsSolution solution)
        {
            foreach (IVsHierarchy hier in GetProjectsInSolution(solution))
            {
                EnvDTE.Project project = hier.GetDTEProject();
                if (project != null)
                {
                    yield return project;
                }
            }
        }

        private static IEnumerable<IVsHierarchy> GetProjectsInSolution(this IVsSolution solution)
        {
            return GetProjectsInSolution(solution, __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION);
        }

        private static IEnumerable<IVsHierarchy> GetProjectsInSolution(
            IVsSolution solution,
            __VSENUMPROJFLAGS flags)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (solution == null)
                yield break;

            IEnumHierarchies enumHierarchies;
            Guid guid = Guid.Empty;
            solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies);
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

        private static EnvDTE.Project GetDTEProject(this IVsHierarchy hierarchy)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (hierarchy == null)
                throw new ArgumentNullException("hierarchy");

            hierarchy.GetProperty(
                (uint)VSConstants.VSITEMID.Root,
                (int)__VSHPROPID.VSHPROPID_ExtObject, out object obj);

            return obj as EnvDTE.Project;
        }

        /// <summary>
        /// Determines whether the solution is loaded.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>
        ///   <c>true</c> if the solution is loaded; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSolutionLoaded(this IVsSolution solution)
        {
            return (solution.GetProjects().Count() > 0);
        }
    }
}
