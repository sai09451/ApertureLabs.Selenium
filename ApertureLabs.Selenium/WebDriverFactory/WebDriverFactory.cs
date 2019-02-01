using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Responsible for creation and disposing of the various webdrivers.
    /// Searches the current directory for the location of the drivers.
    /// </summary>
    public class WebDriverFactory : IDisposable
    {
        #region Fields

        private readonly DriverManager driverManager;
        private readonly IList<IWebDriver> trackedDrivers;
        private readonly SeleniumServerStandAloneManager seleniumServerStandAloneHelper;
        private readonly SeleniumHub seleniumHub;

        private bool disposedValue = false;

        #endregion

        #region Constructor/Finalizer

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactory"/> class.
        /// </summary>
        public WebDriverFactory()
            : this(SeleniumHubOptions.Default(), SeleniumNodeOptions.Default())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactory"/> class.
        /// </summary>
        /// <param name="seleniumHubOptions">The selenium hub options.</param>
        /// <param name="seleniumNodeOptions">The selenium node options.</param>
        /// <exception cref="ArgumentNullException">
        /// seleniumHubOptions
        /// or
        /// seleniumNodeOptions
        /// </exception>
        /// <exception cref="Exception">
        /// Failed to start the hub process.
        /// or
        /// Failed to start the node process.
        /// </exception>
        public WebDriverFactory(
            SeleniumHubOptions seleniumHubOptions,
            SeleniumNodeOptions seleniumNodeOptions)
        {
            if (seleniumHubOptions == null)
                throw new ArgumentNullException(nameof(seleniumHubOptions));
            else if (seleniumNodeOptions == null)
                throw new ArgumentNullException(nameof(seleniumNodeOptions));

            disposedValue = false;
            driverManager = new DriverManager();
            trackedDrivers = new List<IWebDriver>();
            seleniumHub = new SeleniumHub(seleniumHubOptions);

            seleniumHub.Start();
            seleniumHub.RegisterNode(seleniumNodeOptions);
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
            var driverOptions = new RemoteSessionSettings();

            switch (majorWebDriver)
            {
                case MajorWebDriver.Chrome:
                    driverManager.SetUpDriver(new ChromeConfig());
                    driverOptions.AddFirstMatchDriverOption(new ChromeOptions());
                    break;
                case MajorWebDriver.Edge:
                    driverManager.SetUpDriver(new EdgeConfig());
                    driverOptions.AddFirstMatchDriverOption(new EdgeOptions());
                    break;
                case MajorWebDriver.Firefox:
                    driverManager.SetUpDriver(new FirefoxConfig());
                    driverOptions.AddFirstMatchDriverOption(new FirefoxOptions());
                    break;
                case MajorWebDriver.InternetExplorer:
                    driverManager.SetUpDriver(new InternetExplorerConfig());
                    driverOptions.AddFirstMatchDriverOption(new InternetExplorerOptions());
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Set the window size.
            var driver = new RemoteWebDriver(driverOptions);
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

        private bool IsLocalHubRunning()
        {
            return false;
        }

        #region IDisposable Support
        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    seleniumHub?.Dispose();
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
