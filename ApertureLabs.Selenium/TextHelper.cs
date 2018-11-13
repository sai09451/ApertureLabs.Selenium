using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Drawing;
using System.Globalization;
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

        public static Color FromCssString(string cssColor)
        {

        }

        #endregion
    }
}
