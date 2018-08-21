using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;

namespace Aperture.Nop400.PageObjects.Public
{
    /// <summary>
    /// The base page for all 
    /// </summary>
    public class BasePage : PageObject
    {
        #region Fields

        #endregion

        #region Constructor

        public BasePage(IWebDriver driver) : base(driver)
        {
            Uri = new Uri("/", UriKind.Relative);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        #endregion
    }
}
