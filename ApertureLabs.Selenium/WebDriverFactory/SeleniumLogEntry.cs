using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// An entry outputted by the selenium-server-standalone.jar.
    /// </summary>
    public struct SeleniumLogEntry
    {
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is exception.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is exception; otherwise, <c>false</c>.
        /// </value>
        public bool IsException { get; set; }

        /// <summary>
        /// Gets or sets the type of the log.
        /// </summary>
        /// <value>
        /// The type of the log.
        /// </value>
        public string LogType { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Parses the string.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        /// <returns></returns>
        public static SeleniumLogEntry ParseString(string logEntry)
        {
            var result = new SeleniumLogEntry();
            var match = Regex.Match(logEntry, @"(?<datetime>\d+:\d+:\d+.\d+)\s(?<logtype>\w+)\s\[(?<action>.*?)\]\s-\s(?<message>.*)");

            if (match.Success)
            {
                result.Action = match.Groups["action"].Value;
                result.LogType = match.Groups["logtype"].Value;
                result.Message = match.Groups["message"].Value;

                result.DateTime = DateTime.ParseExact(
                    match.Groups["datetime"].Value,
                    "HH:mm:ss.fff",
                    CultureInfo.CurrentCulture);
            }
            else
            {
                result.DateTime = DateTime.MinValue;
                result.IsException = true;
                result.Message = logEntry;
            }

            return result;
        }
    }
}
