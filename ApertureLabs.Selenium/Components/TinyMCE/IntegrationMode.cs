namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// The 'mode' of a TinyMCE editor.
    /// </summary>
    public enum IntegrationMode
    {
        /// <summary>
        /// The form-based mode.
        /// </summary>
        Classic = 0,

        /// <summary>
        /// The inline mode. No iframe is used with the editor.
        /// </summary>
        Inline = 1,

        /// <summary>
        /// The distraction free mode essentially is just the text editor area
        /// without any menus, toolbars, or status bars.
        /// </summary>
        DistractionFree = 2
    }
}
