using ApertureLabs.Selenium.Attributes;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// A PageObject whose url has route parameters.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IParameterPageObject" />
    /// <seealso cref="System.IDisposable" />
    [CodeGeneration]
    public abstract class ParameterPageObject : PageObject, IParameterPageObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterPageObject"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="baseUri">The base uri.</param>
        /// <param name="route">The route.</param>
        public ParameterPageObject(IWebDriver driver,
            Uri baseUri,
            UriTemplate route)
            : base(driver,
                  baseUri,
                  route)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the URI information.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Need to call Load first.</exception>
        public virtual UriTemplateMatch GetUriInfo()
        {
            if (Uri == null)
                throw new Exception("Need to call Load first.");

            var authority = new Uri(Uri.GetLeftPart(UriPartial.Authority));
            var path = new Uri(
                Uri.GetComponents(
                    UriComponents.PathAndQuery,
                    UriFormat.Unescaped));

            return Route.Match(authority, path);
        }

        /// <summary>
        /// Replaces the variables in the <see cref="IPageObject.Route" /> with
        /// the parameters and navigates to that url (if not on it already)
        /// then calls <see cref="ILoadableComponent.Load" />.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the <see cref="IDictionary{TKey, TValue}"/> argument is
        /// null.
        /// </exception>
        public virtual ILoadableComponent Load(IDictionary<string, string> parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            // Need to extract the authority section of the route template.
            var uri = Route.BindByName(BaseUri, parameters);

            // Navigate to the url.
            var alreadyOnUrl = String.Equals(
                uri.ToString(),
                WrappedDriver.Url,
                StringComparison.OrdinalIgnoreCase);

            if (!alreadyOnUrl)
                WrappedDriver.Navigate().GoToUrl(uri);

            return Load();
        }

        #endregion
    }
}
