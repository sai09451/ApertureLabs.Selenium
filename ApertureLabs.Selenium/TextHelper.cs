﻿using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
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
        /// Retrieves and trims the inner text of the element.
        /// </summary>
        public string InnerText => WrappedElement.Text.Trim();

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
                    if (!String.IsNullOrEmpty(format))
                    {
                        var provider = CultureInfo.CurrentCulture;
                        return DateTime.ParseExact(matches.Value,
                            format,
                            provider);
                    }
                    else
                    {
                        return DateTime.Parse(String.Format("{1}/{0}/{2} {3}",
                            day,
                            month,
                            year,
                            time));
                    }
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

        #endregion
    }
}