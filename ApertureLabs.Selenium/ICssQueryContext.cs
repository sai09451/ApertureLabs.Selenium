using ApertureLabs.Selenium.WebElement;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium
{
    public interface ICssQueryContext
    {
        /// <summary>
        /// Searches for a elements matching the selector.
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <param name="wait"></param>
        /// <returns></returns>
        IList<IWebElementV2> Select(string cssSelector, TimeSpan? wait = null);
    }
}
