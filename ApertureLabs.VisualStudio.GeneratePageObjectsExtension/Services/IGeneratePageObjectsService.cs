using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Models;
using EnvDTE;

namespace ApertureLabs.VisualStudio.GeneratePageObjectsExtension.Services
{
    public interface IGeneratePageObjectsService
    {
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

        /// <summary>
        /// Generates the page objects.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        Task GeneratePageObjectsAsync(SynchronizePageObjectsModel model);
    }
}