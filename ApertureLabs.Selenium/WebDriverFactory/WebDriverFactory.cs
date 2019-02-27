using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
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

        private readonly bool isStandalone;
        private readonly DriverManager driverManager;
        private readonly IList<IWebDriver> trackedDrivers;
        private readonly SeleniumServerStandAloneManager seleniumServerStandAloneHelper;
        private readonly SeleniumHub seleniumHub;

        private bool disposedValue = false;

        #endregion

        #region Constructor/Finalizer

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactory"/>
        /// class. NOTE: This will use STANDALONE mode. It won't create a hub
        /// and node. This generally is quicker for running tests sequentially
        /// but slower for parallel stuff.
        /// </summary>
        public WebDriverFactory()
        {
            isStandalone = true;
            disposedValue = false;
            driverManager = new DriverManager();
            trackedDrivers = new List<IWebDriver>();
            seleniumHub = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebDriverFactory"/>
        /// class. NOTE: This will create a hub and node process for better
        /// performance when running tests in parallel.
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

            isStandalone = false;
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
            var driver = default(IWebDriver);
            var driverOpts = default(DriverOptions);
            var driverConfig = default(IDriverConfig);

            switch (majorWebDriver)
            {
                case MajorWebDriver.Chrome:
                    driverConfig = new ChromeConfig();
                    driverOpts = new ChromeOptions();
                    break;
                case MajorWebDriver.Edge:
                    driverConfig = new EdgeConfig();
                    driverOpts = new EdgeOptions();
                    break;
                case MajorWebDriver.Firefox:
                    driverConfig = new FirefoxConfig();
                    driverOpts = new FirefoxOptions();
                    break;
                case MajorWebDriver.InternetExplorer:
                    driverConfig = new InternetExplorerConfig();
                    driverOpts = new InternetExplorerOptions();
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Download driver if not available locally.
            driverManager.SetUpDriver(driverConfig);

            if (isStandalone)
            {
                switch (majorWebDriver)
                {
                    case MajorWebDriver.Chrome:
                        {
                            var opts = driverOpts as ChromeOptions;
                            opts.BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
                            driver = new ChromeDriver(opts);
                            break;
                        }
                    case MajorWebDriver.Edge:
                        {
                            driver = new EdgeDriver();
                            break;
                        }
                    case MajorWebDriver.Firefox:
                        {
                            var opts = driverOpts as FirefoxOptions;
                            opts.BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                            driver = new FirefoxDriver(opts);
                            break;
                        }
                    case MajorWebDriver.InternetExplorer:
                        {
                            driver = new InternetExplorerDriver();
                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                var d = new RemoteSessionSettings();
                d.AddFirstMatchDriverOption(driverOpts);

                driver = new RemoteWebDriver(d);
            }

            // Set the window size.
            driver.Manage().Window.Size = windowSize;

            // Wrap with EventFiringWebDriver.
            driver = new EventFiringWebDriver(driver);

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
                    seleniumHub?.Dispose();

                    foreach (var driver in trackedDrivers)
                    {
                        try
                        {
                            driver.Quit();
                            driver.Dispose();
                        }
                        catch
                        { }
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
