namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Options for the TinyMCEComponent.
    /// </summary>
    public class TinyMCEOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TinyMCEOptions"/> class.
        /// </summary>
        public TinyMCEOptions()
        {
            InteractWithMenuViaJavaScript = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to interact with the menu
        /// using the javascript api. If false then interactions will be
        /// attempted via clicking.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [interact with menu via java script]; otherwise, <c>false</c>.
        /// </value>
        public bool InteractWithMenuViaJavaScript { get; set; }
    }
}
