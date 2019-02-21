using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for parsing information from the text of an element.
    /// </summary>
    public class TextHelper : IWrapsDriver, IWrapsElement
    {
        #region Fields

        private const string InnerTextScript =
            "var el = arguments[0];" +
            "return el.innerText;";

        private const string OuterTextScript =
            "var el = arguments[0];" +
            "return el.outerText;";

        private const string InnerHtmlScript =
            "var el = arguments[0];" +
            "return el.innerHTML;";

        private const string OuterHtmlScript =
            "var el = arguments[0];" +
            "return el.outerHTML;";

        private static readonly IEnumerable<CustomDateTimeFormatString> FormatStrings = new[]
        {
            new CustomDateTimeFormatString{ Specifier = "d", RegexString = @"\d{1}" },
            new CustomDateTimeFormatString{ Specifier = "dd", RegexString = @"\d{2}" },
            new CustomDateTimeFormatString{ Specifier = "ddd", RegexString = @"\w+" },
            new CustomDateTimeFormatString{ Specifier = "dddd", RegexString = @"\w+" },
            new CustomDateTimeFormatString{ Specifier = "f", RegexString = @"\d{1}" },
            new CustomDateTimeFormatString{ Specifier = "ff", RegexString = @"\d{2}" },
            new CustomDateTimeFormatString{ Specifier = "fff", RegexString = @"\d{3}" },
            new CustomDateTimeFormatString{ Specifier = "ffff", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "fffff", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "ffffff", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "fffffff", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "F", Implemented = false },
            new CustomDateTimeFormatString{ Specifier = "FF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "FFF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "FFFF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "FFFFF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "FFFFFF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "FFFFFFF", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "g", Implemented = false },
            new CustomDateTimeFormatString{ Specifier = "gg", Implemented = false },
            new CustomDateTimeFormatString{ Specifier = "h", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "hh", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "H", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "HH", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "K", Implemented = false },
            new CustomDateTimeFormatString{ Specifier = "m", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "mm", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "M", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "MM", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "MMM", RegexString = @"\w+" },
            new CustomDateTimeFormatString{ Specifier = "MMMM", RegexString = @"\w+" },
            new CustomDateTimeFormatString{ Specifier = "s", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "ss", RegexString = @"\d+" },
            new CustomDateTimeFormatString{ Specifier = "t", RegexString = @"\w" },
            new CustomDateTimeFormatString{ Specifier = "tt", RegexString = @"[^\s]" },
            new CustomDateTimeFormatString{ Specifier = "y", RegexString = @"\d{1,2}" },
            new CustomDateTimeFormatString{ Specifier = "yy", RegexString = @"\d{2}" },
            new CustomDateTimeFormatString{ Specifier = "yyy", RegexString = @"\d{3}" },
            new CustomDateTimeFormatString{ Specifier = "yyyy", RegexString = @"\d{4}" },
            new CustomDateTimeFormatString{ Specifier = "yyyyy", RegexString = @"\d{4,5}" },
            new CustomDateTimeFormatString{ Specifier = "z", RegexString = @"[+-]\d{1}" },
            new CustomDateTimeFormatString{ Specifier = "z", RegexString = @"[+-]\d{2}" },
            new CustomDateTimeFormatString{ Specifier = "zzz", RegexString = @"[+-][\d]{1,2}:{2}" },
            new CustomDateTimeFormatString{ Specifier = ":", RegexString = CultureInfo.CurrentCulture.DateTimeFormat.TimeSeparator },
            new CustomDateTimeFormatString{ Specifier = "/", RegexString = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator }
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="element"></param>
        public TextHelper(IWebElement element)
        {
            WrappedElement = element;
            WrappedDriver = element.GetDriver();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Retrieves and trims the inner text of the element.
        /// </summary>
        public string InnerText => WrappedDriver
            .JavaScriptExecutor()
            .ExecuteScript(InnerTextScript, WrappedElement)
            .ToString()
            .Trim();

        /// <summary>
        /// Retrieves and trims the outer text of the element.
        /// </summary>
        public string OuterText => WrappedDriver
            .ExecuteJavaScript<string>(OuterTextScript, WrappedElement)
            .Trim();

        /// <summary>
        /// Returns the trimmed inner html of the element.
        /// </summary>
        public string InnerHtml => WrappedDriver
            .ExecuteJavaScript<string>(InnerHtmlScript, WrappedElement)
            .Trim();

        /// <summary>
        /// Returns the trimmed outer html of the element.
        /// </summary>
        public string OuterHtml => WrappedDriver
            .ExecuteJavaScript<string>(OuterHtmlScript, WrappedElement)
            .Trim();

        /// <summary>
        /// Gets the <see cref="T:OpenQA.Selenium.IWebDriver" /> used to find this element.
        /// </summary>
        public IWebDriver WrappedDriver { get; protected set; }

        /// <summary>
        /// Gets the <see cref="T:OpenQA.Selenium.IWebElement" /> wrapped by this object.
        /// </summary>
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
        /// Returns all numbers in the elements inner text as integers.
        /// </summary>
        /// <param name="roundUp"></param>
        /// <returns></returns>
        public IEnumerable<int> ExtractIntegers(bool roundUp = false)
        {
            var r = new Regex(@"(-?\d+\.?\d*)");
            var match = r.Match(InnerText);

            while ((match?.Success ?? false) && match.Groups.Count >= 2)
            {
                var number = Double.Parse(match.Groups[1].Value);

                if (roundUp)
                    number = Math.Round(number);

                // Yield the number.
                yield return (int)number;

                match = match.NextMatch();
            }
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
        /// Returns all numbers in the inner text as doubles.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> ExtractDoubles()
        {
            var r = new Regex(@"(-?\d+\.?\d*)");
            var match = r.Match(InnerText);

            while ((match?.Success ?? false) && match.Groups.Count >= 2)
            {
                var number = Double.Parse(match.Groups[1].Value);

                // Yield the number.
                yield return number;

                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Extracts a Date from the text. Currently this only supports
        /// the text format "Some text...MM/DD/YYYY...other text". It will
        /// optionally also return the time up to the second if provided after
        /// the date.
        /// </summary>
        /// <param name="format">
        /// Optional format the date is in (IE: "MMMM").
        /// </param>
        /// <returns></returns>
        public DateTime ExtractDateTime(string format = default)
        {
            Regex regex = null;

            if (format == null)
                regex = new Regex(@"([\d]{1,2})\s?[(\/|\-)]\s?([(\d|\-)]{1,2})\s?[(\/|\-)]\s?([\d]{2,4}).?(\d{1,2}\:\d{1,2}\:?(\d{1,2})?\s?(AM|PM|A|P|am|pm|a|p))?", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            else
                regex = BuildRegexFromStringFormat(format);

            var matches = regex.Match(InnerText);

            if (matches.Success)
            {
                var dateStr = matches.Groups[1].Value;

                return DateTime.ParseExact(dateStr,
                    format,
                    CultureInfo.CurrentCulture);
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
            var matches = regex.Match(InnerText);

            if (matches.Groups.Count == 0)
            {
                throw new NotFoundException("Failed to find the last four digits of a credit card (xXXXX) in the text");
            }

            return Int32.Parse(matches.Groups[1].ToString());
        }

        private Regex BuildRegexFromStringFormat(string dateTimeFormat)
        {
            var pattern = String.Empty;

            using (var reader = new StringReader(dateTimeFormat))
            {
                while (-1 != reader.Peek())
                {
                    var @char = (char)reader.Read();
                    string subPattern = @char.ToString();

                    if (FormatStrings.Any(fs => fs.Specifier == subPattern))
                    {
                        // Check if the next character combines with the
                        // current one.
                        do
                        {
                            @char = Convert.ToChar(reader.Read());

                            var newResult = subPattern + @char;

                            if (FormatStrings.Any(fs => fs.Specifier == newResult))
                                subPattern = newResult;
                            else
                                break;
                        }
                        while (-1 != reader.Peek()
                            && FormatStrings.Any(fs => fs.Specifier == subPattern));

                        var formatString = FormatStrings
                            .First(fs => fs.Specifier == subPattern);

                        if (!formatString.Implemented)
                            throw new NotImplementedException(subPattern);

                        // Append the regex onto the pattern.
                        pattern += formatString.RegexString;
                    }
                    else
                    {
                        pattern += @char.ToString();
                    }
                }
            }

            return new Regex($"({pattern})", RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        }

        #endregion

        #region Nested Classes

        private class CustomDateTimeFormatString
        {
            public CustomDateTimeFormatString()
            {
                Implemented = true;
            }

            public string Specifier { get; set; }
            public string RegexString { get; set; }
            public bool Implemented { get; set; }
        }

        #endregion
    }
}
