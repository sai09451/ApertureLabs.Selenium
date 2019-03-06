using System;
using System.Drawing;
using System.Globalization;

namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Represents a css color.
    /// </summary>
    public class CssColor : CssValue
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="value"></param>
        public CssColor(string value) : base(value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException(nameof(value));

            var colorFormat = ColorFormat();

            if (IsCssWideKeyword)
            {
                Color = Color.Transparent;
            }
            else
            {
                switch (colorFormat)
                {
                    case CssColorFormat.BasicColorKeyword:
                        Color = Color.FromName(value);
                        break;
                    case CssColorFormat.CurrentColor:
                    case CssColorFormat.Transparent:
                        Color = Color.Transparent;
                        break;
                    case CssColorFormat.Hexadecimal:
                        ColorFromHex();
                        break;
                    case CssColorFormat.HSL:
                    case CssColorFormat.HSLA:
                        ColorFromHsl();
                        break;
                    case CssColorFormat.RGB:
                    case CssColorFormat.RGBA:
                        ColorFromRgb();
                        break;
                    case CssColorFormat.Unknown:
                    default:
                        throw new NotImplementedException(value);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The color parsed from the value.
        /// </summary>
        public Color Color { get; private set; }

        #endregion

        #region Methods

        private CssColorFormat ColorFormat()
        {
            if (Value.IndexOf('(') > 0)
            {
                // Uses a functional color format.
                var function = new CssFunction(Value);
                var functionName = function.FunctionName;

                switch (functionName)
                {
                    case "hsl":
                        return CssColorFormat.HSL;
                    case "hsla":
                        return CssColorFormat.HSLA;
                    case "rgb":
                        return CssColorFormat.RGB;
                    case "rgba":
                        return CssColorFormat.RGBA;
                    default:
                        return CssColorFormat.Unknown;
                }
            }
            else if (Value.StartsWith("#"))
            {
                // Is a hex format.
                return CssColorFormat.Hexadecimal;
            }
            else if (String.Equals("currentColor", Value, StringComparison.Ordinal))
            {
                return CssColorFormat.CurrentColor;
            }
            else if (String.Equals("transparent", Value, StringComparison.Ordinal))
            {
                return CssColorFormat.Transparent;
            }
            else
            {
                // It's most likely a keyword.
                return CssColorFormat.BasicColorKeyword;
            }
        }

        /// <summary>
        /// Assigns Color from value interperted as an hsl function.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="https://www.w3.org/TR/css-color-3/#hsl-color"/>
        private void ColorFromHsl()
        {
            var function = new CssFunction(Value);
            var functionArgs = function.Arguments;

            //HOW TO RETURN hsl.to.rgb(h, s, l): 
            //   SELECT:
            //      IF l <= 0.5: PUT l*(s + 1) IN m2
            //      ELSE: PUT l+s - l * s IN m2
            //   PUT l*2 - m2 IN m1
            //   PUT hue.to.rgb(m1, m2, h + 1 / 3) IN r
            //   PUT hue.to.rgb(m1, m2, h) IN g
            //   PUT hue.to.rgb(m1, m2, h - 1 / 3) IN b
            //   RETURN(r, g, b)

            //HOW TO RETURN hue.to.rgb(m1, m2, h):
            //   IF h<0: PUT h+1 IN h
            //   IF h> 1: PUT h-1 IN h
            //   IF h*6 < 1: RETURN m1+(m2 - m1) * h * 6
            //   IF h*2 < 1: RETURN m2
            //   IF h*3 < 2: RETURN m1+(m2 - m1) * (2 / 3 - h) * 6
            //   RETURN m1

            double h = 0,
                s = 0,
                l = 0;

            double m1 = 0,
                m2 = 0;

            int r = 0,
                g = 0,
                b = 0,
                a = 255; // Default to full transparency.

            var hCss = new CssDimension(functionArgs[0]);
            var sCss = new CssDimension(functionArgs[1]);
            var lCss = new CssDimension(functionArgs[2]);

            switch (hCss.UnitOfMeasurement)
            {
                case CssUnit.None:
                case CssUnit.Percent:
                    h = Normalize(value: hCss.Number,
                        actualMin: 0,
                        actualMax: 100);
                    break;
                case CssUnit.Degrees:
                    h = Normalize(
                        value: hCss.Number,
                        actualMin: 0,
                        actualMax: 360);
                    break;
                case CssUnit.Radians:
                    h = Normalize(
                        value: hCss.Number,
                        actualMin: 0,
                        actualMax: Math.PI * 2);
                    break;
                case CssUnit.Turn:
                    h = Normalize(
                        value: hCss.Number,
                        actualMin: 0,
                        actualMax: 1);
                    break;
                case CssUnit.Gradians:
                    var asRads = (hCss.Number * (180 / 200)) / .9;
                    h = Normalize(
                        value: hCss.Number,
                        actualMin: 0,
                        actualMax: Math.PI * 2);
                    break;
                default:
                    throw new NotImplementedException(Enum.GetName(
                        typeof(CssUnit),
                        hCss.UnitOfMeasurement));
            }

            // Verify the 's' and 'l' units are in percent.
            if (sCss.UnitOfMeasurement != CssUnit.Percent
                || lCss.UnitOfMeasurement != CssUnit.Percent)
            {
                throw new Exception();
            }

            // Assign 's' and 'l'.
            s = Normalize(
                value: sCss.Number,
                actualMin: 0,
                actualMax: 100,
                desiredMin: 0,
                desiredMax: 1);

            l = Normalize(
                value: lCss.Number,
                actualMin: 0,
                actualMax: 100,
                desiredMin: 0,
                desiredMax: 1);

            // Parse alpha if available.
            if (functionArgs.Count == 4)
            {
                var aCss = new CssDimension(functionArgs[3]);

                if (aCss.UnitOfMeasurement == CssUnit.Percent)
                {
                    a = (int)Normalize(value: aCss.Number,
                        actualMin: 0,
                        actualMax: 100,
                        desiredMin: 0,
                        desiredMax: 255);
                }
                else if (aCss.UnitOfMeasurement == CssUnit.None)
                {
                    a = (int)Normalize(value: aCss.Number,
                        actualMin: 0,
                        actualMax: 1,
                        desiredMin: 0,
                        desiredMax: 255);
                }
            }

            // Local function equivilent to the hue.to.rgb function.
            double hueToRgb(double _h)
            {
                if (_h < 0)
                    _h = _h + 1d;

                if (_h > 1d)
                    _h = _h - 1d;

                if (_h * 6d < 1d)
                    return m1 + (m2 - m1) * _h * 6d;
                else if (_h * 2d < 1d)
                    return m2;
                else if (_h * 3d < 2d)
                    return m1 + (m2 - m1) * (2d / 3d - _h) * 6;
                else
                    return m1;
            }

            if (l <= 0.5)
                m2 = l * (s + 1d);
            else
                m2 = l + s - 1d * s;

            m1 = l * 2 - m2;
            r = (int)hueToRgb(h + (1d / 3d));
            g = (int)hueToRgb(h);
            b = (int)hueToRgb(h - (1d / 3d));

            // Re-normalize r, g, and b.
            r *= 255;
            g *= 255;
            b *= 255;

            Color = Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Creates a color from rgba color string.
        /// </summary>
        /// <returns></returns>
        private void ColorFromRgb()
        {
            var cssFunc = new CssFunction(Value);
            var args = cssFunc.Arguments;

            var a = new CssDimension("255").Number;
            var r = new CssDimension(args[0]).Number;
            var g = new CssDimension(args[1]).Number;
            var b = new CssDimension(args[2]).Number;

            if (args.Count == 4)
                a = new CssDimension(args[3]).Number;

            Color = Color.FromArgb((int)a, (int)r, (int)g, (int)b);
        }

        /// <summary>
        /// Creates a color from a hex string.
        /// </summary>
        /// <returns></returns>
        private void ColorFromHex()
        {
            var arg = Value.Replace("#", "");
            var culture = CultureInfo.GetCultureInfo("en-US");

            // Check if the hex str is abbreviated.
            if (arg.Length == 6)
            {
                // Is six digits.
                Color = Color.FromArgb(255,
                    Int32.Parse(
                        arg.Substring(0, 2),
                        NumberStyles.HexNumber,
                        culture),
                    Int32.Parse(
                        arg.Substring(2, 2),
                        NumberStyles.HexNumber,
                        culture),
                    Int32.Parse(arg.Substring(4, 2),
                        NumberStyles.HexNumber,
                        culture));
            }
            else
            {
                // Is three digits.
                var r = Int32.Parse(
                    arg[0].ToString(culture) + arg[0].ToString(culture),
                    NumberStyles.HexNumber,
                    culture);

                var g = Int32.Parse(
                    arg[1].ToString(culture) + arg[1].ToString(culture),
                    NumberStyles.HexNumber,
                    culture);

                var b = Int32.Parse(
                    arg[2].ToString(culture) + arg[2].ToString(culture),
                    NumberStyles.HexNumber,
                    culture);

                Color = Color.FromArgb(255, r, g, b);
            }
        }

        /// <summary>
        /// Maps a value in a range to another range.
        /// </summary>
        /// <param name="value">The number being mapped.</param>
        /// <param name="actualMin">Ignored if infinity.</param>
        /// <param name="actualMax">Ignored if infinity.</param>
        /// <param name="desiredMin">Defaults to zero.</param>
        /// <param name="desiredMax">Defaults to one.</param>
        /// <returns></returns>
        private double Normalize(double value,
            double actualMin = Double.NegativeInfinity,
            double actualMax = Double.PositiveInfinity,
            double desiredMin = 0,
            double desiredMax = 1)
        {
            if (!Double.IsInfinity(actualMin)
                && !Double.IsInfinity(actualMax))
            {
                value *= ((actualMin == 0 ? 1 : actualMin) / actualMax);
            }

            return value * ((desiredMin == 0 ? 1 : desiredMin) / desiredMax);
        }

        #endregion
    }
}
