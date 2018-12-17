using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for parsing information from the text of an element.
    /// </summary>
    public class TextHelper : IWrapsDriver, IWrapsElement
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public TextHelper(IWebElement element)
        {
            WrappedElement = element;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves the inner text of the element.
        /// </summary>
        public string InnerText => WrappedElement.Text;

        /// <summary>
        /// Returns the inner html of the element.
        /// </summary>
        public string InnerHtml => throw new NotImplementedException();

        /// <summary>
        /// Returns the outer html of the element.
        /// </summary>
        public string OuterHtml => throw new NotImplementedException();

        /// <inheritdoc/>
        public IWebDriver WrappedDriver => WrappedElement.GetDriver();

        /// <inheritdoc/>
        public IWebElement WrappedElement { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts a number from the Text of the element. If the text
        /// of the element is "Some text...-34.32...more text" it will
        /// return -34. It completely ignores the decimal unless the 
        /// optional parameter roundUp is true.
        /// </summary>
        /// <param name="roundUp"></param>
        /// <returns></returns>
        public int ExtractInteger(bool roundUp = false)
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = Int32.Parse(matches.Groups[2].ToString());

            return number;
        }

        /// <summary>
        /// Tries to extract number from the text of the element.
        /// </summary>
        /// <returns></returns>
        public decimal ExtractPrice()
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = Decimal.Parse(matches.Groups[1].ToString());

            return number;
        }

        /// <summary>
        /// Extracts a number from the Text of the element. If the text
        /// of the element is "Some text...-34.32...more text" it will
        /// return -34.32.
        /// </summary>
        /// <returns></returns>
        public double ExtractDouble()
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = Double.Parse(matches.Groups[1].ToString());

            return number;
        }

        /// <summary>
        /// Extracts a Date from the text. Currently this only supports 
        /// the text format "Some text...MM/DD/YYYY...other text". It will
        /// optionally also return the time up to the second if provided after
        /// the date.
        /// </summary>
        /// <returns></returns>
        public DateTime ExtractDateTime()
        {
            string filter = @"([\d]{1,2})\s?[(\/|\-)]\s?([(\d|\-)]{1,2})\s?[(\/|\-)]\s?([\d]{2,4}).?(\d{1,2}\:\d{1,2}\:?(\d{1,2})?\s?(AM|PM|A|P|am|pm|a|p))?";
            var r = new Regex(filter, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            var matches = r.Match(InnerText);

            if (matches.Success)
            {
                var day = matches.Groups[1].Value;
                var month = matches.Groups[2].Value;
                var year = matches.Groups[3].Value;

                var time = matches.Groups?[4]?.Value;
                //var temp = string.Format("{0}/{1}/{2} {3}", day, month, year, time);
                try
                {
                    return DateTime.Parse(String.Format("{1}/{0}/{2} {3}",
                        day,
                        month,
                        year,
                        time));
                }
                catch (FormatException)
                {
                    return DateTime.Parse(String.Format("{0}/{1}/{2} {3}",
                        day,
                        month,
                        year,
                        time));
                }
            }
            else
            {
                throw new InvalidCastException("Couldn't find a date in the text '"
                    + WrappedElement.Text
                    + "'");
            }
        }

        /// <summary>
        /// Fetches the last four digits from a partial credit card number 
        /// in a string. Ex: (blah x4444 blah blah) text will return 4444.
        /// </summary>
        /// <returns></returns>
        public int ExtractLastFourCCDigits()
        {
            var regex = new Regex(@"x(\d{4})");
            var matches = regex.Match(WrappedElement.Text);

            if (matches.Groups.Count == 0)
            {
                throw new NotFoundException("Failed to find the last four digits of a credit card (xXXXX) in the text");
            }

            return Int32.Parse(matches.Groups[1].ToString());
        }

        /// <summary>
        /// Returns the color associated with a rgb string.
        /// </summary>
        /// <param name="rgbStr">Can be RGB or ARGB.</param>
        /// <returns></returns>
        public static Color FromRgbString(string rgbStr)
        {
            var color = default(Color);
            var regex = new Regex(@"rgb\((\d{1,3}), (\d{1,3}), (\d{1,3})\)");
            var matches = regex.Matches(rgbStr);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                var r = int.Parse(groups[1].Value);
                var g = int.Parse(groups[2].Value);
                var b = int.Parse(groups[3].Value);
                var a = 255;

                if (groups.Count == 5)
                {
                    a *= (int)(((double)a) * double.Parse(groups[4].Value));
                }

                color = Color.FromArgb(a, r, g, b);
            }

            return color;
        }

        /// <summary>
        /// Creates a color from a hex string.
        /// </summary>
        /// <param name="hexStr"></param>
        /// <returns></returns>
        public static Color FromHexString(string hexStr)
        {
            var sixDigitRegex = new Regex(@"#([\w]{2})([\w]{2})([\w]{2})");
            var threeDigitRegex = new Regex(@"#([\w]{1})([\w]{1})([\w]{1})");

            int r = 0;
            int g = 0;
            int b = 0;

            // Check if the hex str is abbreviated.
            if (sixDigitRegex.IsMatch(hexStr))
            {
                // Is six digit.
                var matches = sixDigitRegex.Matches(hexStr);

                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;

                    r = int.Parse(groups[1].Value, NumberStyles.HexNumber);
                    g = int.Parse(groups[2].Value, NumberStyles.HexNumber);
                    b = int.Parse(groups[3].Value, NumberStyles.HexNumber);
                }
            }
            else
            {
                // Is three digits.
                var matches = threeDigitRegex.Matches(hexStr);

                foreach (Match match in matches)
                {
                    GroupCollection groups = match.Groups;

                    int fromStr (string str)
                    {
                        return int.Parse(str + str, NumberStyles.HexNumber);
                    }

                    r = fromStr(groups[0].Value);
                    g = fromStr(groups[1].Value);
                    b = fromStr(groups[2].Value);
                }
            }

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Creates a color from html color name.
        /// </summary>
        /// <param name="htmlColor"></param>
        /// <returns></returns>
        public static Color FromHtmlColorString(string htmlColor)
        {
            return Color.FromName(htmlColor);
        }

        /// <summary>
        /// Creates a color from rgba color string.
        /// </summary>
        /// <param name="rgbaString"></param>
        /// <returns></returns>
        public static Color FromRgbaString(string rgbaString)
        {
            var color = default(Color);
            var regex = new Regex(@"rgba\((\d{1,3}), (\d{1,3}), (\d{1,3}), (\d{1,3})\)");
            var matches = regex.Matches(rgbaString);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

                var r = int.Parse(groups[1].Value);
                var g = int.Parse(groups[2].Value);
                var b = int.Parse(groups[3].Value);
                var a = int.Parse(groups[4].Value); ;

                if (groups.Count == 5)
                {
                    a *= (int)(((double)a) * double.Parse(groups[4].Value));
                }

                color = Color.FromArgb(a, r, g, b);
            }

            return color;
        }

        /// <summary>
        /// Creates a Color from hsl.
        /// </summary>
        /// <param name="hslString"></param>
        /// <returns></returns>
        /// <seealso cref="https://www.w3.org/TR/css-color-3/#hsl-color"/>
        public static Color ColorFromHslString(string hslString)
        {
            var color = default(Color);
            var regex = new Regex(@"^hsl\((\d*).?\s*,?\s*(\d*).?\s*,?\s*(.\d*).?\)$");
            var matches = regex.Matches(hslString);

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;

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

                var h = int.Parse(groups[1].Value);
                var s = int.Parse(groups[2].Value);
                var l = int.Parse(groups[3].Value);

                var m1 = 0;
                var m2 = 0;

                int r = 0,
                    g = 0,
                    b = 0;

                // Local function equivilent to the hue.to.rgb function.
                int hueToRgb(int _h)
                {
                    if (_h < 0)
                        _h = _h + 1;

                    if (_h > 1)
                        _h = _h - 1;

                    if (_h * 6 < 1)
                        return m1 + (m2 - m1) * _h * 6;
                    else if (_h * 2 < 1)
                        return m2;
                    else if (_h * 3 < 2)
                        return m1 + (m2 - m1) * (2 / 3 - _h) * 6;
                    else
                        return m1;
                }

                if (l <= 0.5)
                    m2 = l * (s + 1);
                else
                    m2 = l + s - 1 * s;

                m1 = l * 2 - m2;
                r = hueToRgb(h + 1 / 3);
                g = hueToRgb(h);
                b = hueToRgb(h - 1 / 3);

                color = Color.FromArgb(255, r, g, b);

                //double r = 0, g = 0, b = 0;
                //double temp1, temp2;

                //if (l == 0)
                //{
                //    r = g = b = 0;
                //}
                //else
                //{
                //    if (s == 0)
                //    {
                //        r = g = b = l;
                //    }
                //    else
                //    {
                //        temp2 = ((l <= 0.5) ? l * (1.0 + s) : l + s - (l * s));
                //        temp1 = 2.0 * l - temp2;

                //        double[] t3 = new double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
                //        double[] clr = new double[] { 0, 0, 0 };
                //        for (int i = 0; i < 3; i++)
                //        {
                //            if (t3[i] < 0)
                //                t3[i] += 1.0;

                //            if (t3[i] > 1)
                //                t3[i] -= 1.0;

                //            if (6.0 * t3[i] < 1.0)
                //                clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                //            else if (2.0 * t3[i] < 1.0)
                //                clr[i] = temp2;
                //            else if (3.0 * t3[i] < 2.0)
                //                clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                //            else
                //                clr[i] = temp1;
                //        }

                //        r = clr[0];
                //        g = clr[1];
                //        b = clr[2];
                //    }
                //}

                //color = Color.FromArgb(255,
                //    (int)(255 * r),
                //    (int)(255 * g),
                //    (int)(255 * b));
            }

            return color;
        }

        /// <summary>
        /// Convets a css color string to a Color. Recognizes hex, rgb, rgba,
        /// and html color codes.
        /// </summary>
        /// <param name="cssColor"></param>
        /// <returns></returns>
        public static Color FromCssString(string cssColor)
        {
            if (IsHexString(cssColor))
                return FromHexString(cssColor);
            else if (IsRgbString(cssColor))
                return FromRgbString(cssColor);
            else if (IsRgbaString(cssColor))
                return FromRgbaString(cssColor);
            else if (IsHtmlString(cssColor))
                return FromHtmlColorString(cssColor);
            else if (IsHslString(cssColor))
                throw new NotImplementedException();
            else if (IsHslaString(cssColor))
                throw new NotImplementedException();
            else
                throw new ArgumentException($"Failed to determine the format " +
                    $"of the css color string: '{cssColor}'.");
        }

        private static bool IsRgbaString(string cssColorStr)
        {
            throw new NotImplementedException();
        }

        private static bool IsRgbString(string cssColorStr)
        {
            throw new NotImplementedException();
        }

        private static bool IsHexString(string cssColorStr)
        {
            throw new NotImplementedException();

            // var isHexStr = cssColorStr.StartsWith("#")
            //     && (cssColorStr.Length == 4 || cssColorStr.Length == 7);
        }

        private static bool IsHtmlString(string cssColorStr)
        {
            var validColorNames = typeof(Color)
                .GetProperties(BindingFlags.Static)
                .Select(p => p.Name)
                .ToList();

            return validColorNames.Any(n => String.Equals(n,
                cssColorStr,
                StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsHslString(string cssColorStr)
        {
            var c = Color.FromArgb(255, 255, 225, 255);

            throw new NotImplementedException();
        }

        private static bool IsHslaString(string cssColorStr)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
