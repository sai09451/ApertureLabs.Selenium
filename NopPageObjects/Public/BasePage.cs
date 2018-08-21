using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace Aperture.NopPageObjects
{
    /// <summary>
    /// For all classes that inherit from this class remember to call
    /// base.InitElements() in the constructor.
    /// </summary>
    public class BasePage : IPageObject
    {
        #region Constructor

        public BasePage(IWebDriver driver)
        {
            WrappedDriver = driver;
        }

        #endregion

        #region Properties

        public IWebDriver WrappedDriver { get; private set; }

        public Uri Uri { get; private set; }

        #endregion

        #region Methods

        public ILoadableComponent Load()
        {
            throw new NotImplementedException();
        }

        public bool IsStateValid()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
