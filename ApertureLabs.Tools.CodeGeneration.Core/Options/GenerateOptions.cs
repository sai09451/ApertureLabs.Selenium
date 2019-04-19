using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ApertureLabs.Selenium.CodeGeneration;
using ApertureLabs.Tools.CodeGeneration.Core.Services;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    [Verb(
        name: "generate",
        HelpText = "Generates code files.",
        Hidden = false)]
    public class GenerateOptions : LogOptions, ICommand
    {
        #region Properties

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
            longName: "original-project-kind",
            Required = false,
            Default = "Core",
            HelpText = "The dotnet type of the original project. Can be " +
                "Core, Standard, or Framework. Defaults to Core.")]
        public string OriginalProjectKind { get; set; }

        [Option(
            longName: "destination-project-kind",
            Required = false,
            Default = "Standard",
            HelpText = "The dotnet type of the destination project. Can be " +
                "Core, Standard, or Framework. Defaults to Standard.")]
        public string DestinationProjectKind { get; set; }

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

        //[Option(
        //    longName: "overwrite-existing",
        //    Required = false,
        //    Default = false,
        //    HelpText = "Will completely regenerate the resulting projects and files.")]
        //public bool OverwriteExistingFiles { get; set; }

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

            Program.LogSupportedChanges(workspace);

            if (!Enum.TryParse(typeof(FrameworkKind),
                OriginalProjectKind,
                out var originalFrameworkKind))
            {
                throw new InvalidCastException("The argument " +
                    "original-framework-kind had an invalid value.");
            }

            if (!Enum.TryParse(typeof(FrameworkKind),
                DestinationProjectKind,
                out var destinationProjectKind))
            {
                throw new InvalidCastException("The argument " +
                    "destination-framework-kind had an invalid value");
            }

            // Retrieve original and destination projects.
            var originalProject = Program.GetProject(solution,
                OriginalProjectName,
                (FrameworkKind)originalFrameworkKind);
            var destinationProject = Program.GetProject(
                solution,
                DestinationProjectName,
                (FrameworkKind)destinationProjectKind);

            Program.Log.Info($"Original project: {originalProject.Name}");
            Program.Log.Info($"Destination project: {destinationProject.Name}");

            // Compile original project.
            //var compilation = await originalProject.GetCompilationAsync()
            //    .ConfigureAwait(false);

            // TODO: Allow loading code generators via reflection.
            var codeGenerators = new ICodeGenerator[]
            {
                new SeleniumCodeGenerator()
            };

            if (!codeGenerators.Any())
            {
                throw new Exception("No code generators located in the " +
                    "destination assembly.");
            }

            var _progress = new Progress<CodeGenerationProgress>();

            foreach (var codeGenerator in codeGenerators)
            {
                // Generate the code.
                var modifiedDestProj = await codeGenerator.Generate(
                        originalProject,
                        destinationProject,
                        _progress,
                        cancellationToken)
                    .ConfigureAwait(false);

                // Get list of changes.
                Program.LogSolutionChanges(
                    modifiedDestProj.Solution,
                    destinationProject.Solution);

                // Just print out changes if doing a dry run.
                if (DryRun)
                    continue;

                // Prompt for accepting changes.
                Console.Write("Accept changes (Y/N): ");
                var response = Console.ReadLine();

                var isPositiveResponse = response.Equals("y", StringComparison.OrdinalIgnoreCase)
                    || response.Equals("yes", StringComparison.OrdinalIgnoreCase);

                if (!isPositiveResponse)
                {
                    // Exit if not applying the changes.
                    Program.Log.Info("Exiting program");
                    return;
                }

                // Apply changes.
                if (workspace.TryApplyChanges(modifiedDestProj.Solution))
                {
                    // Success.
                    Program.Log.Info("Applied changes successfully");
                }
                else
                {
                    // Error.
                    Program.Log.Error("Failed to apply changes.");
                }

                progress.Report(100);
            }
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
