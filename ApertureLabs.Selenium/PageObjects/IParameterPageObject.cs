using OpenQA.Selenium.Support.UI;
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
        /// Gets the URI information.
        /// </summary>
        /// <returns></returns>
        UriTemplateMatch GetUriInfo();

        /// <summary>
        /// Replaces the variables in the <see cref="IPageObject.Route"/> with
        /// the parameters and navigates to that url (if not on it already)
        /// then calls <see cref="ILoadableComponent.Load"/>.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        ILoadableComponent Load(IDictionary<string, string> parameters);
    }
}
