using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    #region Properties

    [Verb(
        name: "generate",
        HelpText = "Generates code files.",
        Hidden = false)]
    public class GenerateOptions : LogOptions, ICommand
    {
        [Option(
            longName: "original-project-name",
            Required = true,
            HelpText = "The name of the project to generate the code from.")]
        public string OriginalProjectName { get; set; }

        [Option(
            longName: "destination-project-name",
            Required = true,
            HelpText = "The name of the project to place the code in.")]
        public string DestinationProjectName { get; set; }

        [Option(
            longName: "assume-yes",
            Required = false,
            HelpText = "Answers yes to all y/n prompts.")]
        public bool AssumeYes { get; set; }

        [Option(
            longName: "dry-run",
            Required = false,
            Default = false,
            HelpText = "No changes will be saved to disk.")]
        public bool DryRun { get; set; }

        [Option(
            longName: "overwrite-existing",
            Required = false,
            Default = false,
            HelpText = "Will completely regenerate the resulting projects and files.")]
        public bool OverwriteExistingFiles { get; set; }

        [Option(
            longName: "path-to-solution",
            HelpText = "The full or relative path to the solution file (*.sln).",
            Required = false)]
        public string PathToSolution { get; set; }

        #endregion

        #region Methods

        public async Task ExecuteAsync(
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            await Program.InitializeAsync(this).ConfigureAwait(false);

            var (workspace, solution) = await Program
                .GetWorkspaceAndSolution(PathToSolution)
                .ConfigureAwait(false);

            // Retrieve original and destination projects.
            var originalProject = RetrieveOriginalProject(solution);
            var destinationProject = RetrieveOrCreateTargetProject(solution);

            Program.Log.Info($"Original project: {originalProject.Name}");
            Program.Log.Info($"Destination project: {destinationProject.Name}");

            // Compile original project.
            var compilation = await originalProject.GetCompilationAsync()
                .ConfigureAwait(false);

            // Get the location of the built assembly.
            var diagnostics = compilation.GetDiagnostics();

            throw new NotImplementedException();
        }

        private Project RetrieveOriginalProject(Solution solution)
        {
            var project = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    OriginalProjectName,
                    StringComparison.Ordinal));

            if (project == null)
            {
                Program.Log.Error(
                    new Exception($"No such project found with name '{OriginalProjectName}'."),
                    true);
            }

            return project;
        }

        private Project RetrieveOrCreateTargetProject(Solution solution)
        {
            var destinationProject = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    DestinationProjectName,
                    StringComparison.Ordinal));

            if (OverwriteExistingFiles)
            {
                // Remove project.
                if (destinationProject != null)
                    solution = solution.RemoveProject(destinationProject.Id);

                // Create new project.
                destinationProject = CreateProject(solution);
            }
            else
            {
                if (destinationProject == null)
                {
                    // Create new project if it doesn't exist.
                    destinationProject = CreateProject(solution);
                }
            }

            return destinationProject;
        }

        private Project CreateProject(Solution solution)
        {
            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Default,
                DestinationProjectName,
                NamespaceSafeName(),
                "csharp");

            solution = solution.AddProject(projectInfo);
            var project = solution.GetProject(projectInfo.Id);
            InstallNuGetPackages(project);

            return project;
        }

        private string NamespaceSafeName()
        {
            var @namespace = DestinationProjectName.Replace(
                oldValue: " ",
                newValue: "_",
                ignoreCase: false,
                culture: CultureInfo.CurrentCulture);

            // Append a "_" to the namespace if the first letter is a number.
            if (Char.IsNumber(@namespace[0]))
                @namespace = "_" + @namespace;

            return @namespace;
        }

        private static void InstallNuGetPackages(Project project)
        {
            // TODO
            //IFrameworkNameProvider frameworkProvider = new FrameworkNameProvider(
            //    mappings: Array.Empty<IFrameworkMappings>(),
            //    portableMappings: Array.Empty<IPortableFrameworkMappings>());

            //var packageReader = new PackageArchiveReader(filePath: project.FilePath);

            //var installedPackages = PackageHelper.GetInstalledPackageFilesAsync(
            //        packageReader: packageReader,
            //        packageIdentity: null,
            //        packagePathResolver: null,
            //        packageSaveMode: PackageSaveMode.Defaultv2,
            //        cancellationToken: CancellationToken.None)
            //    .Result;

            //var providers = new List<Lazy<INuGetResourceProvider>>();
            //providers.AddRange(Repository.Provider.GetCoreV3());
            ////providers.AddRange(Repository.Provider.GetCoreV2());
            //var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            //var sourceRepo = new SourceRepository(packageSource, providers);
            //var packageMetadataResource = sourceRepo.GetResource<PackageMetadataResource>();
            //var searchMetadata = packageMetadataResource.GetMetadataAsync(
            //        packageId: "ApertureLabs.Selenium",
            //        includePrerelease: false,
            //        includeUnlisted: false,
            //        sourceCacheContext: new SourceCacheContext(),
            //        log: null,
            //        token: CancellationToken.None)
            //    .Result;

            //var settings = (ISettings)new Settings(
            //    root: null,
            //    fileName: null,
            //    isMachineWide: false);

            //var packageSourceProvider = new SourceRepositoryProvider(
            //    packageSourceProvider: new PackageSourceProvider(
            //        settings: settings,
            //        configurationDefaultSources: new[]
            //        {
            //            packageSource
            //        }),
            //    resourceProviders: providers);

            //var packageManager = new NuGetPackageManager(
            //    sourceRepositoryProvider: packageSourceProvider,
            //    settings: settings,
            //    solutionManager: null,
            //    deleteOnRestartManager: null,
            //    excludeVersion: false);
        }

        #endregion
    }
}
