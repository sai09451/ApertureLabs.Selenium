using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Represents a web-page which has a url with parameters.
    /// </summary>
    public interface IPageObject : IWrapsDriver,
        ILoadableComponent,
        IDisposable,
        IEquatable<IPageObject>
    {
        /// <summary>
        /// Used to match the Uri of the webpage. Is used to create a Regex
        /// object to verify the url of the page when calling <c>Load</c>.
        /// Identical to the RouteAttributes parameter.
        /// NOTE: All meta-sequences should be in groups (preferrably named
        /// groups).
        /// </summary>
        string UriMatcher { get; }

        /// <summary>
        /// Gets the URI. Should be null until
        /// <see cref="OpenQA.Selenium.Support.UI.ILoadableComponent.Load"/> is
        /// called where it will be set to the Url the
        /// <see cref="OpenQA.Selenium.IWebDriver" />
        /// is on.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the window handle the page was originally loaded on.
        /// </summary>
        string WindowHandle { get; }

        /// <summary>
        /// Use to determine if a PageObject is still 'valid'.
        /// </summary>
        /// <example>
        /// If there are two page object instances; one for a login page and
        /// one for the home page, and the driver is on the home page the login
        /// page instance should return true while the home page instance
        /// should return true.
        /// </example>
        /// <returns></returns>
        bool IsStale();
    }
}
