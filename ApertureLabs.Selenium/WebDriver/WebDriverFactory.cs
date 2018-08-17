using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.WebDriver
{
    public enum MajorWebDriver
    {
        CHROME,
        EDGE,
        FIREFOX
    }

    public class WebDriverFactory
    {
        private static IList<IWebDriver> ActiveWebDrivers
            = new List<IWebDriver>();

        #region Constructor
        public WebDriverFactory()
        {

        }
        #endregion

        #region Properties

        public Uri WebDriverLocations { get; set; }

        #endregion

        #region Methods

        public WebDriverV2 GetDriver(string filename)
        {
            throw new NotImplementedException();
        }

        public WebDriverV2 GetDriver(MajorWebDriver majorWebDriver)
        {
            switch (majorWebDriver)
            {
                case MajorWebDriver.CHROME:
                    return GetDriver("chromedriver");
                case MajorWebDriver.EDGE:
                    return GetDriver("edgedriver");
                case MajorWebDriver.FIREFOX:
                    return GetDriver("firefox");
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
