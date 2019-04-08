using Microsoft.CodeAnalysis;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core.Services
{
    /// <summary>
    /// Contains methods for installing/searching/uninstalling NuGet packages.
    /// </summary>
    public class NuGetUtilities
    {
        #region Fields

        #endregion

        #region Constructor

        public NuGetUtilities()
        { }

        #endregion

        #region Methods

        public async Task InstallPackageAsync(
            Project project,
            string packageId,
            string version,
            string framework)
        {
            var packageIdentity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
            var settings = Settings.LoadDefaultSettings(root: null);
            var sourceRepoProvider = new SourceRepositoryProvider(settings, Repository.Provider.GetCoreV3());
            var nuGetFramework = NuGetFramework.ParseFolder(framework);
            var logger = NullLogger.Instance;

            using (var cacheContext = new SourceCacheContext())
            {
                var repositories = sourceRepoProvider.GetRepositories();
                var availablePackages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);

                // This will cache the dependencies.
                await GetPackageDependencies(
                        packageIdentity,
                        nuGetFramework,
                        cacheContext,
                        logger,
                        repositories,
                        availablePackages)
                    .ConfigureAwait(false);

                var resolverContext = new PackageResolverContext(
                    dependencyBehavior: DependencyBehavior.Lowest,
                    targetIds: new[] { packageId },
                    requiredPackageIds: Enumerable.Empty<string>(),
                    packagesConfig: Enumerable.Empty<PackageReference>(),
                    preferredVersions: Enumerable.Empty<PackageIdentity>(),
                    availablePackages: availablePackages,
                    packageSources: sourceRepoProvider
                        .GetRepositories()
                        .Select(s => s.PackageSource),
                    log: logger);

                var resolver = new PackageResolver();
                var packagesToInstall = resolver.Resolve(
                        resolverContext,
                        CancellationToken.None)
                    .Select(
                        p => availablePackages.Single(
                            x => PackageIdentityComparer.Default.Equals(x, p)));

                var packagePathResolver = new PackagePathResolver(Path.GetFullPath("packages"));
                var packageExtractContext = new PackageExtractionContext(
                    PackageSaveMode.Defaultv3,
                    XmlDocFileSaveMode.None,
                    ClientPolicyContext.GetClientPolicy(settings, logger),
                    //new PackageSignatureVerifier(
                        //new[] { new SignatureTrustAndValidityVerificationProvider() as ISignatureVerificationProvider }),
                        //SignatureVerificationProviderFactory.GetSignatureVerificationProviders()),
                    logger);
                var frameworkReducer = new FrameworkReducer();

                foreach (var packageToInstall in packagesToInstall)
                {
                    PackageReaderBase packageReader;
                    var installedPath = packagePathResolver.GetInstallPath(packageToInstall);

                    if (installedPath == null)
                    {
                        var downloadResource = await packageToInstall.Source
                            .GetResourceAsync<DownloadResource>(CancellationToken.None)
                            .ConfigureAwait(false);

                        var downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                                packageToInstall,
                                new PackageDownloadContext(cacheContext),
                                SettingsUtility.GetGlobalPackagesFolder(settings),
                                logger,
                                CancellationToken.None)
                            .ConfigureAwait(false);

                        await PackageExtractor.ExtractPackageAsync(
                                downloadResult.PackageSource,
                                downloadResult.PackageStream,
                                packagePathResolver,
                                packageExtractContext,
                                CancellationToken.None)
                            .ConfigureAwait(false);

                        packageReader = downloadResult.PackageReader;
                    }
                    else
                    {
                        packageReader = new PackageFolderReader(installedPath);
                    }

                    var libtItems = packageReader.GetLibItems();
                    var nearest = frameworkReducer.GetNearest(
                        nuGetFramework,
                        libtItems.Select(x => x.TargetFramework));

                    var frameworkItems = packageReader.GetFrameworkItems();
                    nearest = frameworkReducer.GetNearest(
                        nuGetFramework,
                        frameworkItems.Select(x => x.TargetFramework));
                }
            }

            // Update csproj file to include the project reference.
            var packageRef = MetadataReference.CreateFromFile(
                path: String.Empty,
                properties: MetadataReferenceProperties.Assembly);
            var updatedProject = project.AddMetadataReference(packageRef);

            if (!project.Solution.Workspace.TryApplyChanges(updatedProject.Solution))
            {
                throw new Exception("Failed to apply changes.");
            }
        }

        private async Task GetPackageDependencies(PackageIdentity package,
            NuGetFramework framework,
            SourceCacheContext cacheContext,
            ILogger logger,
            IEnumerable<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package)) return;

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository
                    .GetResourceAsync<DependencyInfoResource>()
                    .ConfigureAwait(false);
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                        package,
                        framework,
                        cacheContext,
                        logger,
                        CancellationToken.None)
                    .ConfigureAwait(false);

                if (dependencyInfo == null) continue;

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await GetPackageDependencies(
                            new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                            framework,
                            cacheContext,
                            logger,
                            repositories,
                            availablePackages)
                        .ConfigureAwait(false);
                }
            }
        }

        #endregion
    }
}
