using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Represents a web-page whose url contians parameters.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IPageObject" />
    public interface IParameterPageObject : IPageObject
    {
        /// <summary>
        /// Gets the URL parameters.
        /// </summary>
        /// <returns></returns>
        IEnumerable<RouteParameter> GetUrlParameters();

        /// <summary>
        /// Gets the parameter information. Attempts to cast the result to the
        /// type parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        T GetParameterInfo<T>(string parameterName) where T : IConvertible;
    }
}
