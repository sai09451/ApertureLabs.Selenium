using System;

namespace ApertureLabs.Selenium.PageObjects
{
    public interface IPageObject
    {
        /// <summary>
        /// The url of the webpage.
        /// </summary>
        Uri PageUrl { get; set; }

        /// <summary>
        /// Navigate to a url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool GoToUrl(string url);

        /// <summary>
        /// Navigates to the url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool GoToUrl(Uri url);

        /// <summary>
        /// Reloads the page.
        /// </summary>
        void ReloadPage(bool clearCache = false);
    }
}
