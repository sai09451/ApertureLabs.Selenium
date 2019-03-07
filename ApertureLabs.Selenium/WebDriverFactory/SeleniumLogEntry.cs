using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// An entry outputted by the selenium-server-standalone.jar.
    /// </summary>
    public struct SeleniumLogEntry : IEquatable<SeleniumLogEntry>
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is SeleniumLogEntry log ? Equals(log) : false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap.
            {
                int hash = 17;
                hash = hash * 23 + Action?.GetHashCode() ?? 0;
                hash = hash * 23 + DateTime.GetHashCode();
                hash = hash * 23 + IsException.GetHashCode();
                hash = hash * 23 + LogType?.GetHashCode() ?? 0;
                hash = hash * 23 + Message?.GetHashCode() ?? 0;

                return hash;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(SeleniumLogEntry left, SeleniumLogEntry right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SeleniumLogEntry left, SeleniumLogEntry right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(SeleniumLogEntry other)
        {
            return GetHashCode() == other.GetHashCode();
        }
    }
}
