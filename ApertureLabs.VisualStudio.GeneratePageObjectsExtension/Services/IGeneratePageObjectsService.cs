using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using EnvDTE;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services
{
    public interface IGeneratePageObjectsService
    {
        /// <summary>
        /// Gets the synchronize model.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        SynchronizePageObjectsModel GetSyncModel(Project project);

        /// <summary>
        /// Initializes the service asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task InitializeServiceAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Installs the aperture libraries in project.
        /// </summary>
        /// <param name="project">The project.</param>
        void InstallApertureLibrariesInProject(Project project);
    }
}