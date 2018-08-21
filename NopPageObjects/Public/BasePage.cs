using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;

namespace Aperture.NopPageObjects
{
    /// <summary>
    /// For all classes that inherit from this class remember to call
    /// base.InitElements() in the constructor.
    /// </summary>
    public class BasePage<T>
    {
        #region Constructor

        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }

        #endregion

        #region Properties

        public IWebDriver Driver { get; set; }

        public Uri Uri { get; set; }

        public T Model { get; set; }

        #endregion

        #region Methods

        protected void InitElements()
        {
            //PageFactory.InitElements(Driver, this);
        }

        #endregion
    }
}
