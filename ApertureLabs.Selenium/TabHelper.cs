using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for switching between tabs.
    /// </summary>
    public class TabHelper : IWrapsDriver
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        public TabHelper(IWebDriver driver)
        {
            WrappedDriver = driver;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public IWebDriver WrappedDriver { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the index of the current tab in relation of the other
        /// tabs on the window.
        /// </summary>
        /// <returns></returns>
        public int GetIndexOfCurrentTab()
        {
            return WrappedDriver.WindowHandles.IndexOf(WrappedDriver.CurrentWindowHandle);
        }

        /// <summary>
        /// Returns the number of tabs on the current window.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<string> GetNumberOfTabs()
        {
            return WrappedDriver.WindowHandles.ToList();
        }

        /// <summary>
        /// Opens a new tab in the window.
        /// </summary>
        /// <param name="switchToTab"></param>
        /// <returns></returns>
        public string CreateNewTab(bool switchToTab = false)
        {
            var currentNumberOfHandles = GetNumberOfTabs().Count;
            WrappedDriver.CreateActions()
                .SendKeys(WrappedDriver.Select("body").First(), Keys.LeftControl + "t")
                .Build()
                .Perform();

            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromSeconds(30));
            wait.Until((d) =>
            {
                return d.WindowHandles.Count == (currentNumberOfHandles + 1);
            });

            if (switchToTab)
            {
                WrappedDriver.SwitchTo().Window(WrappedDriver
                    .WindowHandles[WrappedDriver.WindowHandles.Count - 1]);
            }

            return WrappedDriver.WindowHandles[WrappedDriver.WindowHandles.Count - 1];
        }

        #endregion
    }
}
