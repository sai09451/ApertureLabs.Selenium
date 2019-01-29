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

        private static ManualResetEvent Signal = new ManualResetEvent(false);

        private readonly IList<string> hubLogs;
        private readonly IList<string> nodeLogs;
        private readonly Process hubProcess;
        private readonly Process nodeProcess;
        private readonly DriverManager driverManager;
        private readonly IList<IWebDriver> trackedDrivers;

        private bool disposedValue = false;
        private Uri nodeRegisterUrl;
        private Uri nodeUrl;

        #endregion

        #region Constructor/Finalizer

        /// <summary>
        /// Ctor.
        /// </summary>
        public WebDriverFactory()
        {
            disposedValue = false;
            driverManager = new DriverManager();
            hubLogs = new List<string>();
            nodeLogs = new List<string>();
            trackedDrivers = new List<IWebDriver>();

            // Make sure the selenium standalone server jar file is isntalled.
            if (!HasStandaloneJarFile())
                InstallStandaloneJarFile();

            // Use the latest version if multiple are installed.
            var standaloneJarFileName = GetLatestStandaloneFileName();

            // Start up hub.
            var hubStartupInfo = new ProcessStartInfo
            {
                Arguments = $"-jar {standaloneJarFileName} -role hub",
                FileName = "java",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            hubProcess = Process.Start(hubStartupInfo);
            hubProcess.ErrorDataReceived += HubProcess_OutputDataReceived;
            hubProcess.OutputDataReceived += HubProcess_Log;
            hubProcess.ErrorDataReceived += HubProcess_Log;
            hubProcess.BeginOutputReadLine();
            hubProcess.BeginErrorReadLine();

            // Wait for hub to start.
            Signal.WaitOne(TimeSpan.FromSeconds(30));
            hubProcess.ErrorDataReceived -= HubProcess_OutputDataReceived;

            if (hubProcess.HasExited)
            {
                Dispose();
                throw new Exception("Failed to start the hub process.");
            }

            // Start up hub node.
            var nodeStartupInfo = new ProcessStartInfo
            {
                Arguments = $"-jar {standaloneJarFileName} -role node -hub {nodeRegisterUrl}",
                FileName = "java",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            nodeProcess = Process.Start(nodeStartupInfo);
            nodeProcess.ErrorDataReceived += LocalNodeProcess_OutputDataReceived;
            nodeProcess.OutputDataReceived += NodeProcess_Log;
            nodeProcess.ErrorDataReceived += NodeProcess_Log;
            nodeProcess.BeginOutputReadLine();
            nodeProcess.BeginErrorReadLine();

            // Wait for the node to start.
            Signal.WaitOne(TimeSpan.FromSeconds(30));
            nodeProcess.ErrorDataReceived -= LocalNodeProcess_OutputDataReceived;

            if (nodeProcess.HasExited)
            {
                Dispose();
                throw new Exception("Failed to start the node process.");
            }
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

        #region Event Listeners

        private void NodeProcess_Log(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
                nodeLogs.Add(e.Data);
        }

        private void HubProcess_Log(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
                hubLogs.Add(e.Data);
        }

        private void HubProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Contains("Nodes should register to") ?? false)
            {
                var match = Regex.Match(
                    e.Data,
                    @"Nodes should register to\s+(.*)");

                nodeRegisterUrl = new Uri(match.Groups[1].Value);

                Signal.Set();
            }
        }

        private void LocalNodeProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data?.Contains("Selenium Server is up and running on port") ?? false)
            {
                var match = Regex.Match(
                    e.Data,
                    @"Selenium Server is up and running on port.*?(\d+)");
                var port = match.Groups[1];
                nodeUrl = new Uri($"http://localhost:{port}");
            }

            if (e.Data?.Contains("The node is registered to the hub and ready to use") ?? false)
            {
                Signal.Set();
            }
        }

        #endregion

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

        private string GetLatestStandaloneFileName()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(
                workingDirectory,
                "selenium-server-standalone-*.jar");

            if (!files.Any())
                return null;

            var comparer = new KeyComparer(this);
            var latestVersion = files
                .OrderByDescending(file => file, comparer)
                .First();

            return latestVersion;
        }

        private bool HasStandaloneJarFile()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var files = Directory.GetFiles(
                workingDirectory,
                "selenium-server-standalone-*.jar");

            return files.Any();
        }

        private void InstallStandaloneJarFile()
        {
            using (var client = new WebClient())
            {
                var xmlStr = client.DownloadString("http://selenium-release.storage.googleapis.com/");
                var xml = XDocument.Parse(xmlStr);
                var comparer = new KeyComparer(this);

                var (x, y, z) = xml.Root.Elements(XName.Get(
                        "Contents",
                        "http://doc.s3.amazonaws.com/2006-03-01"))
                    .Where(element =>
                    {
                        var value = element.Element(
                                XName.Get(
                                    "Key",
                                    "http://doc.s3.amazonaws.com/2006-03-01"))
                            .Value;

                        var isStandalone = value.Contains(
                            "selenium-server-standalone");

                        var isBeta = value.Contains("beta");

                        return isStandalone && !isBeta;
                    })
                    .Select(element =>
                    {
                        var value = element.Element(
                                XName.Get(
                                    "Key",
                                    "http://doc.s3.amazonaws.com/2006-03-01"))
                            .Value;

                        var version = GetXYZVerion(value);

                        return version;
                    })
                    .OrderByDescending(version => version, comparer)
                    .First();

                var downloadUrl = String.Format(
                    "http://selenium-release.storage.googleapis.com/{0}.{1}/selenium-server-standalone-{0}.{1}.{2}.jar",
                    x,
                    y,
                    z);

                client.DownloadFile(
                    downloadUrl,
                    $"selenium-server-standalone-{x}.{y}.{z}.jar");
            }
        }

        private (int X, int Y, int Z) GetXYZVerion(string str)
        {
            // Ignore beta releasese.
            var versionPart = Regex.Match(
                str,
                @"selenium-server-standalone-(?<x>.*?)\.(?<y>.*?)\.(?<z>.*?)\.jar");

            var xStr = versionPart.Groups["x"].Value;
            var yStr = versionPart.Groups["y"].Value;
            var zStr = versionPart.Groups["z"].Value;

            var x = int.Parse(xStr);
            var y = int.Parse(yStr);
            var z = int.Parse(zStr);

            return (x, y, z);
        }

        #region IDisposable Support
        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    hubProcess?.Close();
                    hubProcess?.WaitForExit(30 * 1000);
                    hubProcess?.Dispose();
                    nodeProcess?.Close();
                    nodeProcess?.WaitForExit(30 * 1000);
                    nodeProcess?.Dispose();

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

        #region Nested Classes

        private class KeyComparer : IComparer<(int X, int Y, int Z)>,
            IComparer<string>
        {
            private readonly WebDriverFactory parent;

            public KeyComparer(WebDriverFactory parent)
            {
                this.parent = parent;
            }

            public int Compare((int X, int Y, int Z) x, (int X, int Y, int Z) y)
            {
                if (x.X == y.X && x.Y == y.Y && x.Z == y.Z)
                    return 0;

                if (x.X != y.X)
                    return x.X > y.X ? 1 : -1;
                else if (x.Y != y.Y)
                    return x.Y > y.Y ? 1 : -1;
                else if (x.Z != y.Z)
                    return x.Z > y.Z ? 1 : -1;

                return 0;
            }

            public int Compare(string x, string y)
            {
                if (x == y)
                    return 0;

                var xVersion = parent.GetXYZVerion(x);
                var yVersion = parent.GetXYZVerion(y);

                return Compare(xVersion, yVersion);
            }
        }

        #endregion
    }
}
