namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Options for the TinyMCEComponent.
    /// </summary>
    public class TinyMCEOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to interact with the menu
        /// using the javascript api. If false then interactions will be
        /// attempted via clicking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [interact with menu via java script]; otherwise, <c>false</c>.
        /// </value>
        public bool InteractWithMenuViaJavaScript { get; set; }

        /// <summary>
        /// Creates a new instance with all properties preset to their
        /// defaults.
        /// </summary>
        /// <returns></returns>
        public static TinyMCEOptions Default()
        {
            return new TinyMCEOptions
            {
                InteractWithMenuViaJavaScript = true
            };
        }
    }
}
