namespace ApertureLabs.Selenium.Components.Kendo
{
    /// <summary>
    /// Options for the KDropDownComponent.
    /// </summary>
    public class BaseKendoConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseKendoConfiguration"/> class.
        /// </summary>
        public BaseKendoConfiguration()
        {
            ControlWithKeyboardInsteadOfMouse = false;
            DataSource = new DataSourceOptions();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the mouse or the
        /// keyboard to interact with the element. The mouse may still be used
        /// to for some interactions where the keyboard will not suffice.
        /// </summary>
        public bool ControlWithKeyboardInsteadOfMouse { get; set; }

        /// <summary>
        /// Gets or sets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        public DataSourceOptions DataSource { get; set; }
    }
}
