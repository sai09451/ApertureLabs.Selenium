using System;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Represents a route parameter. Format should adhere to the
    /// RouteAttribute format.
    /// </summary>
    public class RouteParameter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteParameter"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentException">
        /// Failed to identify the name of the parameter.
        /// </exception>
        public RouteParameter(string parameter)
        {
            var match = Regex.Match(
                parameter,
                @"\{(?<name>\w+):?(?<type>\w+)?(?<optional>\?)?\}");

            if (!match.Success)
                throw new ArgumentException();

            IsOptional = match.Groups["optional"].Success;
            Type = match.Groups["type"]?.Value ?? "string";
            Name = match.Groups["name"]?.Value
                ?? throw new ArgumentException("Failed to identify the name of the parameter.");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is optional.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is optional; otherwise, <c>false</c>.
        /// </value>
        public bool IsOptional { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        #endregion
    }
}
