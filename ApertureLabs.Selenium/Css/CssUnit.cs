namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Represents a css unit of measurement.
    /// </summary>
    /// <remarks>
    /// View https://drafts.csswg.org/css-values-4 for more details.
    /// </remarks>
    public enum CssUnit
    {
        /// <summary>
        /// There is no unit associated with the number.
        /// </summary>
        None,

        #region Absolute Lengths

        /// <summary>
        /// ('cm')
        /// </summary>
        Centimeters,

        /// <summary>
        /// ('mm')
        /// </summary>
        Millimeters,

        /// <summary>
        /// ('Q')
        /// </summary>
        QuarterMillimeters,

        /// <summary>
        /// ('in')
        /// </summary>
        Inches,

        /// <summary>
        /// ('pc')
        /// </summary>
        Picas,

        /// <summary>
        /// ('pt')
        /// </summary>
        Points,

        /// <summary>
        /// ('px')
        /// </summary>
        Pixels,

        #endregion

        #region Angles

        /// <summary>
        /// ('deg') Degrees. There are 360 degrees in a full circle.
        /// </summary>
        Degrees,

        /// <summary>
        /// ('grad') Gradians, also known as "gons" or "grades". There are 400
        /// gradians in a full circle.
        /// </summary>
        Gradians,

        /// <summary>
        /// ('rad') Radians. There are 2π radians in a full circle.
        /// </summary>
        Radians,

        /// <summary>
        /// ('turn') Turns. There is 1 turn in a full circle.
        /// </summary>
        Turn,

        #endregion

        #region Durations

        /// <summary>
        /// ('s') Seconds.
        /// </summary>
        Seconds,

        /// <summary>
        /// ('ms') Milliseconds. There are 1000 milliseconds in a second.
        /// </summary>
        Milliseconds,

        #endregion

        #region Frequencies

        /// <summary>
        /// ('Hz') Hertz. It represents the number of occurrences per second.
        /// </summary>
        Hertz,

        /// <summary>
        /// ('kHz') KiloHertz. A kiloHertz is 1000 Hertz.
        /// </summary>
        KiloHertz,

        #endregion

        #region Relative Lengths

        /// <summary>
        /// ('%') Varies based on which style property it's being used with.
        /// </summary>
        Percent,

        /// <summary>
        /// ('em') Relative to the font size of the element.
        /// </summary>
        EM,

        /// <summary>
        /// ('ex') X-height of the element’s font.
        /// </summary>
        EX,

        /// <summary>
        /// ('cap') Relative to the cap height (the nominal height of capital
        /// letters) of the element’s font.
        /// </summary>
        CAP,

        /// <summary>
        /// ('ch') Relative to the average character advance of a narrow glyph
        /// in the element’s font, as represented by the “0” (ZERO, U+0030)
        /// glyph.
        /// </summary>
        CH,

        /// <summary>
        /// ('ic') Relative to the average character advance of a fullwidth
        /// glyph in the element’s font, as represented by the “水” (CJK water
        /// ideograph, U+6C34) glyph.
        /// </summary>
        IC,

        /// <summary>
        /// ('rem') Relative to the font size of the root element.
        /// </summary>
        REM,

        /// <summary>
        /// ('lh') Relative to the line height of the element.
        /// </summary>
        LineHeight,

        /// <summary>
        /// ('rlh') Relative to the line height of the root element.
        /// </summary>
        RootLineHeight,

        /// <summary>
        /// ('vw') Equal to 1% of the width of the initial containing block.
        /// </summary>
        ViewWidth,

        /// <summary>
        /// ('vh') Equal to 1% of the height of the initial containing block.
        /// </summary>
        ViewHeight,

        /// <summary>
        /// ('vi') Equal to 1% of the size of the initial containing block in
        /// the direction of the root element’s inline axis.
        /// </summary>
        ViewInline,

        /// <summary>
        /// ('vb') Equal to 1% of the size of the initial containing block in
        /// the direction of the root element’s block axis.
        /// </summary>
        ViewBlock,

        /// <summary>
        /// ('vmin') Equal to the smaller 'vw' or 'vh'.
        /// </summary>
        ViewMinimum,

        /// <summary>
        /// ('vmax') Equal to the larger 'vw' or 'vh'.
        /// </summary>
        ViewMaximum,

        #endregion

        #region Resolutions

        /// <summary>
        /// ('dpi') Dots per inch.
        /// </summary>
        DotsPerInch,

        /// <summary>
        /// ('dpcm') Dots per centimeter.
        /// </summary>
        DotsPerCentimeter,

        /// <summary>
        /// ('dppx' | 'x') Dots per 'px' unit.
        /// </summary>
        DotsPerPixelUnit

        #endregion
    }
}
