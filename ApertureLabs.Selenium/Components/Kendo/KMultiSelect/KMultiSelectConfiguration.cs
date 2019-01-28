namespace ApertureLabs.Selenium.Components.Kendo.KMultiSelect
{
    /// <summary>
    /// Configuration and interaction options for the widget.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.Components.Kendo.BaseKendoConfiguration" />
    public class KMultiSelectConfiguration : BaseKendoConfiguration
    {
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

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public KMultiSelectConfiguration Default()
        {
            return new KMultiSelectConfiguration
            {
                AnimationOptions = KMultiSelectAnimationOptions.Default(),
                AutoClose = true
            };
        }
    }
}
