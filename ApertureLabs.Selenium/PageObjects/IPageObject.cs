using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Represents a webpage.
    /// </summary>
    public interface IPageObject : IWrapsDriver, ILoadableComponent, IDisposable
    {
        /// <summary>
        /// The url of the webpage.
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
        /// one for the home page, if the driver is on the home page the login
        /// page should when its <code>loginPage.IsStateValid();</code> should
        /// return false while the <code>homePage.IsStateValid();</code> will
        /// return true.
        /// </example>
        /// <returns></returns>
        bool IsStale();
    }
}
