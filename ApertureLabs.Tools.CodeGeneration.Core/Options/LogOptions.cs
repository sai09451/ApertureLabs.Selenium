using CommandLine;

namespace ApertureLabs.Tools.CodeGeneration.Core.Options
{
    public class LogOptions
    {
        [Option(
            longName: "no-color",
            Default = false,
            HelpText = "Disables colors in the console.",
            Required = false)]
        public bool NoColor { get; set; }

        [Option(
            longName: "structured-output",
            Default = false,
            HelpText = "Will disable all animations and only output " +
                "information once the command has finished running.",
            Required = false)]
        public bool StructuredOutput { get; set; }
    }
}
