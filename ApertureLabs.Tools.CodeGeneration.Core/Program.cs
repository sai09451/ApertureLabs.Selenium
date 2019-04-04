using ApertureLabs.Tools.CodeGeneration.Core.Options;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core
{
    public static class Program
    {
        private static ConsoleLogger Log;

        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<InfoOptions, GenerateOptions>(args)
                .MapResult(
                    (InfoOptions infoOpts) => InfoOptsWithExitCode(infoOpts),
                    (GenerateOptions generateOpts) => GenerateOptsWithExitCode(generateOpts),
                    errors => 1);

            return result;
        }

        static int InfoOptsWithExitCode(InfoOptions infoOptions)
        {
            InitLogger(infoOptions);

            var solutionTask = GetWorkspaceAndSolution(infoOptions.PathToSolution);
            solutionTask.Wait();
            var workspace = solutionTask.Result.Item1;
            var solution = solutionTask.Result.Item2;

            foreach (var project in solution.Projects)
                Log.Info($"Found project: {project.Name}");

            workspace.Dispose();

            return 0;
        }

        static int GenerateOptsWithExitCode(GenerateOptions generateOptions)
        {
            var changes = new List<string>();
            InitLogger(generateOptions);

            var (workspace, solution) = GetWorkspaceAndSolution(
                    generateOptions.PathToSolution)
                .Result;

            var originalProject = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    generateOptions.OriginalProjectName,
                    StringComparison.Ordinal));

            if (originalProject == null)
            {
                Log.Error(
                    new Exception($"No such project found with name '{generateOptions.OriginalProjectName}'."),
                    true);
            }

            var destinationProject = solution.Projects.FirstOrDefault(
                p => p.Name.Equals(
                    generateOptions.DestinationProjectName,
                    StringComparison.Ordinal));

            if (generateOptions.OverwriteExistingFiles)
            {
                // Remove project.
                if (destinationProject != null)
                    solution = solution.RemoveProject(destinationProject.Id);

                // Create new project.
                destinationProject = CreateProject(
                    solution,
                    generateOptions.DestinationProjectName);
            }
            else
            {
                if (destinationProject == null)
                {
                    // Create new project if it doesn't exist.
                    destinationProject = CreateProject(
                        solution,
                        generateOptions.DestinationProjectName);
                }
            }

            Log.Info($"Original project: {originalProject.Name}");
            Log.Info($"Destination project: {destinationProject.Name}");

            if (!workspace.TryApplyChanges(solution))
            {
                Log.Error(
                    new Exception("Failed to update the solution with the project."),
                    true);
            }

            // Retrieve all code generators.
            var codeGenerators = GetAllCodeGenerators(originalProject);

            if (codeGenerators.Any())
            {
                // Create instance of the code generator.
            }

            workspace.Dispose();

            return 0;
        }

        private static IEnumerable<Document> GetDocuments(Project project,
            IEnumerable<string> extensions)
        {
            var results = new List<Document>();

            foreach (var document in project.Documents)
            {
                var extension = Path.GetExtension(document.Name);

                if (extensions.Contains(extension))
                {
                    results.Add(document);
                }
            }

            return results;
        }

        private static Project CreateProject(Solution solution,
            string projectName)
        {
            var projectInfo = ProjectInfo.Create(
                ProjectId.CreateNewId(),
                VersionStamp.Default,
                projectName,
                projectName,
                "csharp");

            solution = solution.AddProject(projectInfo);
            var project = solution.GetProject(projectInfo.Id);
            InstallNuGetPackages(project);

            return project;
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

        private static Task<(MSBuildWorkspace, Solution)> GetWorkspaceAndSolution(
            string pathToSolution = null)
        {
            MSBuildLocator.RegisterDefaults();
            var solutionFile = GetSolutionFile(pathToSolution);
            var workspace = MSBuildWorkspace.Create();

            var progressBar = Log.IndeterminateProgressBar<ProjectLoadProgress>(
                converter: e => $"{Path.GetFileName(e.FilePath)} - {e.TargetFramework ?? "Framework"} - {e.ElapsedTime.TotalSeconds}s",
                name: "Loading solution");

            return workspace.OpenSolutionAsync(
                    solutionFilePath: solutionFile,
                    progress: progressBar,
                    cancellationToken: CancellationToken.None)
                .ContinueWith(t =>
                {
                    return (workspace, t.Result);
                }, TaskScheduler.Default);
        }

        private static IEnumerable<object> GetAllCodeGenerators(Project project)
        {
            var compilation = project.GetCompilationAsync().Result;
            var codeGeneratorInterface = compilation.GetTypeByMetadataName(
                "ApertureLabs.Selenium.CodeGeneration.ICodeGenerator");

            if (codeGeneratorInterface == null)
                return Enumerable.Empty<object>();

            var codeGeneratorTypes = compilation
                .Assembly
                .TypeNames
                .Select(tn => compilation.Assembly.GetTypeByMetadataName(tn))
                .Where(t => t.AllInterfaces.Contains(codeGeneratorInterface)
                    && !t.IsAbstract
                    && t.TypeKind == TypeKind.Class)
                .ToList();

            return codeGeneratorTypes;
        }

        private static void InitLogger(LogOptions logOptions)
        {
            Log = new ConsoleLogger(logOptions);
        }

        private static string GetSolutionFile(string pathToSolution = null)
        {
            if (!String.IsNullOrEmpty(pathToSolution))
            {
                if (File.Exists(pathToSolution))
                {
                    return pathToSolution;
                }
                else
                {
                    Log.Error(
                        new FileNotFoundException(pathToSolution),
                        true);
                }
            }

            var currentDirectory = Directory.GetCurrentDirectory();

            // Look for a file ending with .sln.
            foreach (var file in Directory.EnumerateFiles(currentDirectory))
            {
                var extension = Path.GetExtension(file);
                var isSolution = extension.Equals(
                    ".sln",
                    StringComparison.Ordinal);

                if (!isSolution)
                    continue;

                return file;
            }

            Log.Error(
                new FileNotFoundException("Failed to find the solution file"),
                true);

            return String.Empty;
        }
    }
}
