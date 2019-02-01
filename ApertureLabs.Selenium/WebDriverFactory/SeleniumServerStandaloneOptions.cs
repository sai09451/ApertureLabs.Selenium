namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Common options for both the hub and node options.
    /// </summary>
    public class SeleniumServerStandaloneOptions
    {
        /// <summary>
        /// Gets or sets the filename of the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        public string Log { get; set; }

        /// <summary>
        /// Gets or sets the name of the jar file which will be used to
        /// execute the file.
        /// </summary>
        /// <value>
        /// The name of the jar file.
        /// </value>
        public string JarFileName { get; set; }
    }
}
