using System;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Represents a 'number' value for css.
    /// </summary>
    public class CssDimension : CssValue
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="value"></param>
        public CssDimension(string value) : base(value)
        {
            if (!IsCssWideKeyword)
            {
                var regex = new Regex(@"(\d*\.?\d*)(.*)");
                var match = regex.Match(value);

                var numberStr = match.Groups[1].Value;
                var unit = match.Groups[2].Value;

                Number = Double.TryParse(numberStr, out var number)
                    ? number
                    : Double.NaN;

                UnitOfMeasurement = ParseUnit(unit);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The value as a number.
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// The unit of measurement.
        /// </summary>
        public CssUnit UnitOfMeasurement { get; private set; }

        #endregion

        #region Methods

        private CssUnit ParseUnit(string unit)
        {
            switch (unit)
            {
                case null:
                case "":
                    return CssUnit.None;
                case "cm":
                    return CssUnit.Centimeters;
                case "mm":
                    return CssUnit.Millimeters;
                case "Q":
                    return CssUnit.QuarterMillimeters;
                case "in":
                    return CssUnit.Inches;
                case "pc":
                    return CssUnit.Picas;
                case "pt":
                    return CssUnit.Points;
                case "px":
                    return CssUnit.Pixels;
                case "deg":
                    return CssUnit.Degrees;
                case "grad":
                    return CssUnit.Gradians;
                case "rad":
                    return CssUnit.Radians;
                case "turn":
                    return CssUnit.Turn;
                case "s":
                    return CssUnit.Seconds;
                case "ms":
                    return CssUnit.Milliseconds;
                case "Hz":
                    return CssUnit.Hertz;
                case "kHz":
                    return CssUnit.KiloHertz;
                case "%":
                    return CssUnit.Percent;
                case "em":
                    return CssUnit.EM;
                case "ex":
                    return CssUnit.EX;
                case "cap":
                    return CssUnit.CAP;
                case "ch":
                    return CssUnit.CH;
                case "ic":
                    return CssUnit.IC;
                case "rem":
                    return CssUnit.REM;
                case "lh":
                    return CssUnit.LineHeight;
                case "rlh":
                    return CssUnit.RootLineHeight;
                case "vw":
                    return CssUnit.ViewWidth;
                case "vh":
                    return CssUnit.ViewHeight;
                case "vi":
                    return CssUnit.ViewInline;
                case "vb":
                    return CssUnit.ViewBlock;
                case "vmin":
                    return CssUnit.ViewMinimum;
                case "vmax":
                    return CssUnit.ViewMaximum;
                case "dpi":
                    return CssUnit.DotsPerInch;
                case "dpcm":
                    return CssUnit.DotsPerCentimeter;
                case "dppx":
                    return CssUnit.DotsPerPixelUnit;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
