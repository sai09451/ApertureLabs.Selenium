using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.WebElement
{
    public class TextHelper : IWrapsDriver, IWrapsElement
    {
        #region Fields

        #endregion

        #region Constructor

        public TextHelper(IWebElement element)
        {
            this.WrappedElement = element;
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

        public IWebDriver WrappedDriver => WrappedElement.GetDriver();

        public IWebElement WrappedElement { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Extracts a number from the Text of the element. If the text
        /// of the element is "Some text...-34.32...more text" it will
        /// return -34. It completely ignores the decimal unless the 
        /// optional parameter roundUp is true.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int ExtractInteger(bool roundUp = false)
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = int.Parse(matches.Groups[2].ToString());

            return number;
        }

        /// <summary>
        /// Extracts a number from the Text of the element. If the text
        /// of the element is "Some text...-34.32...more text" it will
        /// return -34.32.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public double ExtractFloatingPointNumber()
        {
            var r = new Regex(@"^.*?((-?\d+)(.\d+)?)");
            var matches = r.Match(InnerText);

            var number = double.Parse(matches.Groups[1].ToString());

            return number;
        }

        /// <summary>
        /// Extracts a Date from the text. Currently this only supports 
        /// the text format "Some text...MM/DD/YYYY...other text". It will
        /// optionally also return the time up to the second if provided after
        /// the date.
        /// </summary>
        /// <param name="element"></param>
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

                string time = matches.Groups?[4]?.Value;
                //var temp = string.Format("{0}/{1}/{2} {3}", day, month, year, time);
                try
                {
                    return DateTime.Parse(string.Format("{1}/{0}/{2} {3}",
                        day,
                        month,
                        year,
                        time));
                }
                catch (FormatException)
                {
                    return DateTime.Parse(string.Format("{0}/{1}/{2} {3}",
                        day,
                        month,
                        year,
                        time));
                }
            }
            else
                throw new InvalidCastException("Couldn't find a date in the text '"
                    + WrappedElement.Text
                    + "'");
        }

        /// <summary>
        /// Fetches the last four digits from a partial credit card number 
        /// in a string. Ex: (blah x4444 blah blah) text will return 4444.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int ExtractLastFourCCDigits()
        {
            var regex = new Regex(@"x(\d{4})");
            var matches = regex.Match(WrappedElement.Text);

            if (matches.Groups.Count == 0)
                throw new NotFoundException("Failed to find the last four digits of a credit card (xXXXX) in the text");

            return int.Parse(matches.Groups[1].ToString());
        }

        #endregion
    }
}
