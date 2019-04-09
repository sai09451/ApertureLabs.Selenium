using ApertureLabs.Tools.CodeGeneration.Core.Options;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ApertureLabs.Tools.CodeGeneration.Core
{
    public enum FrameworkKind
    {
        Framework,
        Standard,
        Core
    }

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
                    errors => Task.CompletedTask);

            try
            {
                // Wait for task to complete.
                task.Wait();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return;
        }

        public static Project GetProject(Solution solution,
            string assemblyName,
            FrameworkKind frameworkKind)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            var project = default(Project);
            var projects = solution.Projects.Where(
                    p => p.AssemblyName.Equals(
                        assemblyName,
                        StringComparison.Ordinal))
                .ToList();

            var filterFor = "net";

            switch (frameworkKind)
            {
                case FrameworkKind.Core:
                    filterFor = "netcoreapp";
                    break;
                case FrameworkKind.Standard:
                    filterFor = "netstandard";
                    break;
            }

            if (projects.Count >= 2)
            {
                var matchingKindsOfProjects = new Dictionary<double, Project>();

                // Get all projects of the same kind.
                foreach (var p in projects)
                {
                    var (framework, version) = GetFrameworkFromProjectName(p.Name);

                    if (framework.Equals(filterFor, StringComparison.Ordinal))
                        matchingKindsOfProjects.Add(version, p);
                }

                // Use the highest version.
                var maxKey = matchingKindsOfProjects.Keys.Max();
                project = matchingKindsOfProjects[maxKey];
            }
            else
            {
                project = projects.FirstOrDefault();
            }

            if (project == null)
                throw new Exception("Failed to find the project.");

            return project;
        }

        private static (string, double) GetFrameworkFromProjectName(string projectName)
        {
            var match = Regex.Match(
                projectName,
                @"^.*?\((\D+)(.*?)\)$");

            var framework = match.Groups[1].Value;
            var version = Double.Parse(
                match.Groups[2].Value,
                CultureInfo.CurrentCulture);

            return (framework, version);
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
