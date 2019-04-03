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
            longName: "project-name",
            Required = true,
            HelpText = "The name of the project to generate the code.")]
        public string ProjectName { get; set; }

        [Option(
            longName: "assume-yes",
            Required = false,
            HelpText = "Answers yes to all y/n prompts.")]
        public bool AssumeYes { get; set; }
    }
}
