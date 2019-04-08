namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Used to display information on how far along a code generation task is.
    /// </summary>
    public class CodeGenerationProgress
    {
        /// <summary>
        /// The total number of steps.
        /// </summary>
        public int TotalSteps { get; set; }

        /// <summary>
        /// The current step. Must be greater than zero and less than or equal
        /// to <see cref="TotalSteps"/>.
        /// </summary>
        public int CurrentStep { get; set; }

        /// <summary>
        /// An optional message to display.
        /// </summary>
        public string Message { get; set; }
    }
}
