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
            return GetTabHandles()
                .IndexOf(WrappedDriver.CurrentWindowHandle);
        }

        /// <summary>
        /// Identical to <see cref="IWebDriver.WindowHandles"/>.
        /// </summary>
        /// <returns></returns>
        public IList<string> GetTabHandles()
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
            var handleCount = GetTabHandles().Count;
            WrappedDriver.CreateActions()
                .SendKeys(WrappedDriver.Select("body").First(), Keys.LeftControl + "t")
                .Build()
                .Perform();

            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromSeconds(30));
            WrappedDriver
                .Wait(TimeSpan.FromSeconds(30))
                .Until((d) => d.WindowHandles.Count == (handleCount + 1));

            if (switchToTab)
            {
                WrappedDriver.SwitchTo().Window(WrappedDriver
                    .WindowHandles[WrappedDriver.WindowHandles.Count - 1]);
            }

            return WrappedDriver.WindowHandles[WrappedDriver.WindowHandles.Count - 1];
        }

        /// <summary>
        /// Switches to next tab.
        /// </summary>
        public void SwitchToNextTab()
        {
            WrappedDriver.CreateActions()
                .SendKeys(
                    WrappedDriver.Select("body").First(),
                    Keys.LeftControl + Keys.Tab)
                .Build()
                .Perform();
        }

        /// <summary>
        /// Switches to previous tab.
        /// </summary>
        public void SwitchToPreviousTab()
        {
            WrappedDriver.CreateActions()
                .SendKeys(
                    WrappedDriver.Select("body").First(),
                    Keys.LeftControl + Keys.LeftShift + Keys.Tab)
                .Build()
                .Perform();
        }

        /// <summary>
        /// Switches to tab.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        public void SwitchToTab(string windowHandle)
        {
            WrappedDriver.SwitchTo().Window(windowHandle);
        }

        #endregion
    }
}
