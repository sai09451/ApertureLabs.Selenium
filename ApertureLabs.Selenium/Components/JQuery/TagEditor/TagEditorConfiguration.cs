namespace ApertureLabs.Selenium.Components.JQuery.TagEditor
{
    /// <summary>
    /// Configuration class for the <see cref="TagEditorComponent{T}"/> class.
    /// </summary>
    public class TagEditorConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditorConfiguration"/> class.
        /// </summary>
        public TagEditorConfiguration()
        {
            UseKeyboardInsteadOfMouseWhenInteracting = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use keyboard instead of
        /// mouse when interacting with the component.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to use keyboard instead of mouse when interacting
        ///   with the component; otherwise, <c>false</c>.
        /// </value>
        public bool UseKeyboardInsteadOfMouseWhenInteracting { get; set; }
    }
}
