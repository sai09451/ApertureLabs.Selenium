using ApertureLabs.Tools.CodeGeneration.Core.Options;
using CommandLine;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.IO;
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
            InitLogger(generateOptions);

            var solution = GetWorkspaceAndSolution();

            return 0;
        }

        private static Task<(MSBuildWorkspace, Solution)> GetWorkspaceAndSolution(
            string pathToSolution = null)
        {
            MSBuildLocator.RegisterDefaults();
            var solutionFile = GetSolutionFile(pathToSolution);
            var workspace = MSBuildWorkspace.Create();

            var progressBar = Log.IndeterminateProgressBar<ProjectLoadProgress>(
                converter: e => $"{Path.GetFileName(e.FilePath)} - {e.TargetFramework}",
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
