using System;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    [Verb(
        name: "info",
        HelpText = "Displays info about the code generation process.",
        Hidden = false)]
    public class InfoOptions : LogOptions, ICommand
    {
        [Option(
            longName: "list-generators",
            Default = true,
            HelpText = "Lists the full type names of all code generators in a project.",
            Required = false)]
        public bool ListCodeGenerators { get; set; }

        [Option(
            longName: "path-to-solution",
            HelpText = "The full or relative path to the solution file (*.sln).",
            Required = false)]
        public string PathToSolution { get; set; }

        public Task ExecuteAsync(
            IProgress<double> progress,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
