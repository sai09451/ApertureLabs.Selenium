using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.WebDriver
{
    public class TabHelper
    {
        #region Fields

        private readonly WebDriverWrapper driver;

        #endregion

        #region Constructor

        public TabHelper(WebDriverWrapper driver)
        {
            this.driver = driver;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// Returns the index of the current tab in relation of the other
        /// tabs on the window.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public int GetIndexOfCurrentTab()
        {
            return driver.WebDriver.WindowHandles
                .IndexOf(driver.WebDriver.CurrentWindowHandle);
        }

        /// <summary>
        /// Returns the number of tabs on the current window.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public IList<string> GetNumberOfTabs()
        {
            return driver.WebDriver.WindowHandles.ToList();
        }

        /// <summary>
        /// Opens a new tab in the window.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="switchToTab"></param>
        /// <returns></returns>
        public string CreateNewTab(bool switchToTab = false)
        {
            var action = driver.CreateAction();
            var currentNumberOfHandles = GetNumberOfTabs();
            action
                .SendKeys(driver.Select("body"), Keys.LeftControl + "t")
                .Build()
                .Perform();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until((d) =>
            {
                return d.WindowHandles.Count == (currentNumberOfHandles + 1);
            });

            if (switchToTab)
                driver.SwitchTo().Window(driver.WindowHandles[driver.WindowHandles.Count - 1]);

            return driver.WindowHandles[driver.WindowHandles.Count - 1];
        }

        #endregion
    }
}
