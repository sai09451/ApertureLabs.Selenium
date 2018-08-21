using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApertureLabs.Selenium.WebDriver
{
    public class TabHelper
    {
        #region Fields

        private readonly WebDriverV2 driver;

        #endregion

        #region Constructor

        public TabHelper(WebDriverV2 driver)
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
            return driver.GetNativeWebDriver().WindowHandles
                .IndexOf(driver.GetNativeWebDriver().CurrentWindowHandle);
        }

        /// <summary>
        /// Returns the number of tabs on the current window.
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public IList<string> GetNumberOfTabs()
        {
            return driver.GetNativeWebDriver().WindowHandles.ToList();
        }

        /// <summary>
        /// Opens a new tab in the window.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="switchToTab"></param>
        /// <returns></returns>
        public string CreateNewTab(bool switchToTab = false)
        {
            var nativeDriver = driver.GetNativeWebDriver();
            var action = driver.CreateAction();
            var currentNumberOfHandles = GetNumberOfTabs().Count;
            action
                .SendKeys(driver.Select("body").First().WebElement, Keys.LeftControl + "t")
                .Build()
                .Perform();

            var wait = new WebDriverWait(nativeDriver, TimeSpan.FromSeconds(30));
            wait.Until((d) =>
            {
                return d.WindowHandles.Count == (currentNumberOfHandles + 1);
            });

            if (switchToTab)
            {
                nativeDriver.SwitchTo().Window(nativeDriver
                    .WindowHandles[nativeDriver.WindowHandles.Count - 1]);
            }

            return nativeDriver
                .WindowHandles[nativeDriver.WindowHandles.Count - 1];
        }

        #endregion
    }
}
