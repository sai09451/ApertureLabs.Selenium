using System;
using System.Collections.Generic;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Enum of common web drivers.
    /// </summary>
    public enum MajorWebDriver
    {
        /// <summary>
        /// Chrome.
        /// </summary>
        Chrome,

        /// <summary>
        /// Edge.
        /// </summary>
        Edge,

        /// <summary>
        /// Firefox.
        /// </summary>
        Firefox,

        /// <summary>
        /// Internet explorer.
        /// </summary>
        InternetExplorer
    }

    /// <summary>
    /// Responsible for creation and disposing of the various webdrivers.
    /// Searches the current directory for the location of the drivers.
    /// </summary>
    public class WebDriverFactory : IDisposable
    {
        #region Fields

        private readonly DriverManager driverManager;
        private readonly IList<IWebDriver> trackedDrivers;

        private bool disposedValue = false;

        #endregion

        #region Constructor/Finalizer

        /// <summary>
        /// Ctor.
        /// </summary>
        public WebDriverFactory()
        {
            disposedValue = false;
            driverManager = new DriverManager();
            trackedDrivers = new List<IWebDriver>();
        }

        /// <summary>
        /// Dtor
        /// </summary>
        ~WebDriverFactory()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new WebDriver instance for one of the major web
        /// browsers.
        /// </summary>
        /// <param name="majorWebDriver"></param>
        /// <param name="windowSize"></param>
        /// <param name="track">
        /// If false, will not dispose the driver when the factory is
        /// disposed.
        /// </param>
        /// <returns></returns>
        public IWebDriver CreateDriver(MajorWebDriver majorWebDriver,
            Size windowSize,
            bool track = true)
        {
            IWebDriver driver = null;

            switch (majorWebDriver)
            {
                case MajorWebDriver.Chrome:
                    driverManager.SetUpDriver(new ChromeConfig());
                    driver = new ChromeDriver();
                    break;
                case MajorWebDriver.Edge:
                    driverManager.SetUpDriver(new EdgeConfig());
                    driver = new EdgeDriver();
                    break;
                case MajorWebDriver.Firefox:
                    var ffConfig = new FirefoxConfig();
                    driverManager.SetUpDriver(new FirefoxConfig());
                    driver = new FirefoxDriver();
                    break;
                case MajorWebDriver.InternetExplorer:
                    driverManager.SetUpDriver(new InternetExplorerConfig());
                    driver = new InternetExplorerDriver();
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Set the window size.
            driver.Manage().Window.Size = windowSize;

            if (track)
                trackedDrivers.Add(driver);

            return driver;
        }

        /// <summary>
        /// Allows tracking of an externally created webdriver.
        /// </summary>
        /// <param name="driver">The driver.</param>
        public void TrackDriver(IWebDriver driver)
        {
            if (trackedDrivers.Contains(driver))
                return;

            trackedDrivers.Add(driver);
        }

        #region IDisposable Support
        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var driver in trackedDrivers)
                    {
                        driver.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        /// <inheritdoc/>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool
            // disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
    }
}
