namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Configuration and interaction options for the widget.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoConfiguration" />
    public class KMultiSelectConfiguration : BaseKendoConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KMultiSelectConfiguration"/> class.
        /// </summary>
        public KMultiSelectConfiguration()
        {
            AnimationOptions = new KMultiSelectAnimationOptions();
            AutoClose = true;
        }

        /// <summary>
        /// Gets or sets the animation options.
        /// </summary>
        /// <value>
        /// The animation options.
        /// </value>
        public KMultiSelectAnimationOptions AnimationOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the widget will
        /// automatically close after selecting an item.
        /// </summary>
        public bool AutoClose { get; set; }
    }
}
