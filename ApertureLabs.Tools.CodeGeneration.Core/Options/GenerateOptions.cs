using CommandLine;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    [Verb(
        name: "generate",
        HelpText = "Generates code files.",
        Hidden = false)]
    public class GenerateOptions : LogOptions
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
    }
}
