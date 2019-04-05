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
        public static ConsoleLogger Log { get; set; }

        [STAThread]
        static void Main(string[] args)
        {
            var progress = new Progress<double>();

            var task = Parser.Default.ParseArguments<TestOptions, InfoOptions, GenerateOptions>(args)
                .MapResult(
                    (TestOptions testOpts) => testOpts.ExecuteAsync(progress, CancellationToken.None),
                    (InfoOptions infoOpts) => infoOpts.ExecuteAsync(progress, CancellationToken.None),
                    (GenerateOptions generateOpts) => generateOpts.ExecuteAsync(progress, CancellationToken.None),
                    errors => HandleErrors(errors));

            // Wait for task to complete.
            task.Wait();

            return;
        }

        static Task HandleErrors(IEnumerable<Error> errors)
        {
            var errorStr = String.Join(
                Environment.NewLine,
                errors.Select(e => e.ToString()));

            return Task.Run(() => Log.Error(new Exception(errorStr), true));
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

        public static Task<(MSBuildWorkspace, Solution)> GetWorkspaceAndSolution(
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

        public static Task InitializeAsync(LogOptions logOptions)
        {
            Log = new ConsoleLogger(logOptions);

            return Task.CompletedTask;
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
