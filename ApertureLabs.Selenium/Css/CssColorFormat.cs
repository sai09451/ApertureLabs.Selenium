namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// The supported color formats.
    /// </summary>
    public enum CssColorFormat
    {
        /// <summary>
        /// Unable to determine the color format.
        /// </summary>
        Unknown,

        /// <summary>
        /// The value of the color property. Similar to one of the css-wide
        /// keywords like auto or inherit.
        /// </summary>
        CurrentColor,

        /// <summary>
        /// The value is of the list of html color keywords.
        /// </summary>
        BasicColorKeyword,

        /// <summary>
        /// The value is transparent.
        /// </summary>
        Transparent,

        /// <summary>
        /// Value is in the hex format. Can be #fff or #ffffff.
        /// </summary>
        Hexadecimal,

        /// <summary>
        /// Value is in the rgb(...) format.
        /// </summary>
        RGB,

        /// <summary>
        /// Value is in the rgba(...) format.
        /// </summary>
        RGBA,

        /// <summary>
        /// Value is in the hsl(...) format.
        /// </summary>
        HSL,

        /// <summary>
        /// Value is in the hsla(...) format.
        /// </summary>
        HSLA
    }
}
