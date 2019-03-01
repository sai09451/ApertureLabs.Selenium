using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Represents a selenium browser session.
    /// </summary>
    public interface ISeleniumSession
    { }

    /// <summary>
    /// SeleniumHub.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class SeleniumHub :
        SeleniumServerStandaloneWrapper<SeleniumHubOptions>
    {
        #region Fields

        private readonly IList<string> hubLogs;
        private readonly IList<SeleniumNode> registeredNodes;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        private AutoResetEvent signal;
        private FileSystemWatcher logWatcher;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumHub"/> class.
        /// </summary>
        public SeleniumHub()
            : this(SeleniumHubOptions.Default())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumHub"/> class.
        /// </summary>
        /// <param name="seleniumHubOptions">The selenium hub options.</param>
        public SeleniumHub(SeleniumHubOptions seleniumHubOptions)
            : base(seleniumHubOptions)
        {
            this.signal = new AutoResetEvent(false);
            this.hubLogs = new List<string>();
            this.registeredNodes = new List<SeleniumNode>();

            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ObjectCreationHandling = ObjectCreationHandling.Auto
            };

            // Default url.
            Uri = new Uri("http://127.0.0.1:4444/");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the node register URL.
        /// </summary>
        /// <value>
        /// The node register URL.
        /// </value>
        public Uri NodeRegisterUrl { get; private set; }

        #endregion

        #region Methods

        #region Event Listeners

        private void LogWatcher_Log(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var fileInfo = new FileInfo(options.Log);

            if (fileInfo.FullName != e.FullPath)
                return;

            // Read & parse the last line.
            var lastLine = File.ReadLines(fileInfo.FullName).Last();
            var logEntry = SeleniumLogEntry.ParseString(lastLine);

            hubLogs.Add(lastLine);
        }

        private void LogWatcher_OutputDataRecieved(
            object sender,
            FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var fileInfo = new FileInfo(options.Log);

            if (fileInfo.FullName != e.FullPath)
                return;

            // Read & parse the last line.
            var lastLine = File.ReadLines(fileInfo.FullName).Last();
            var logEntry = SeleniumLogEntry.ParseString(lastLine);

            // Ignore exception messages.
            if (logEntry.IsException)
                return;

            // Check if the hub is ready.
            if (lastLine.Contains("Nodes should register to"))
            {
                var match = Regex.Match(
                    lastLine,
                    @"Nodes should register to\s+(.*)");

                NodeRegisterUrl = new Uri(match.Groups[1].Value);

                signal.Set();
            }
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

                NodeRegisterUrl = new Uri(match.Groups[1].Value);

                signal.Set();
            }
        }

        #endregion

        /// <summary>
        /// Registers the node. Will over-write the
        /// <c>SeleniumNodeOptions.Hub</c> property with the hubs
        /// 'register node' url.
        /// </summary>
        /// <param name="seleniumNodeOptions">The selenium node.</param>
        /// <returns>The created selenium node.</returns>
        public SeleniumNode RegisterNode(SeleniumNodeOptions seleniumNodeOptions)
        {
            if (seleniumNodeOptions == null)
                throw new ArgumentNullException(nameof(seleniumNodeOptions));
            else if (wrappedProcess == null)
                throw new Exception("Hub hasn't been started yet.");

            // Assign the register url to the hub property.
            seleniumNodeOptions.Hub = NodeRegisterUrl.ToString();
            var seleniumNode = new SeleniumNode(seleniumNodeOptions);
            seleniumNode.Start();
            registeredNodes.Add(seleniumNode);

            return seleniumNode;
        }

        /// <summary>
        /// Uns the register node.
        /// </summary>
        /// <param name="seleniumNode">The selenium node.</param>
        public void UnregisterNode(SeleniumNode seleniumNode)
        {
            if (!registeredNodes.Contains(seleniumNode))
                return;

            seleniumNode.Stop();
        }

        /// <summary>
        /// Gets the registered nodes.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<SeleniumNode> GetRegisteredNodes()
        {
            return registeredNodes
                .ToList()
                .AsReadOnly();
        }

        /// <summary>
        /// Gets the sessions.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ISeleniumSession> GetSessions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <returns></returns>
        public IHubStatus GetStatus()
        {
            var status = default(HubStatus);
            var uri = new Uri(Uri, "grid/api/hub/status?configuration=free,busy,nodes");
            uri.GetLeftPart(UriPartial.Query);

            using (var client = new WebClient())
            {
                var json = client.DownloadString(uri);
                status = JsonConvert.DeserializeObject<HubStatus>(
                    json,
                    jsonSerializerSettings);
            }

            return status;
        }

        /// <summary>
        /// Shuts down the hub and all registered nodes.
        /// </summary>
        public override void Stop()
        {
            if (!IsRunning())
                return;

            foreach (var node in registeredNodes)
                node.Stop();

            registeredNodes.Clear();

            if (options.Servlets.Contains(DefaultServletNames.LifeCycleServlet))
            {
                // Send shutdown request.
                var shutDownUri = new Uri(Uri, "lifecycle-manager?action=shutdown");

                using (var httpClient = new HttpClient())
                {
                    httpClient
                        .GetAsync(shutDownUri)
                        .Wait(TimeSpan.FromSeconds(10));
                }

                wrappedProcess.WaitForExit(10000);
            }
            else
            {
                // Terminate the hub.
                wrappedProcess.Kill();
                wrappedProcess.WaitForExit(10000);
                wrappedProcess.Dispose();
            }

            wrappedProcess = null;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="Exception">Failed to start the hub process.</exception>
        public override void Start()
        {
            // Check if port is busy.
            var port = options.PortNumber
                ?? ExtractPort(options.Host)
                ?? 4444;

            if (IsLocalPortBusy(port, TimeSpan.FromSeconds(1)))
                throw new Exception($"Port {port} wasn't available.");

            var useLog = !String.IsNullOrEmpty(options.Log);

            var hubStartupInfo = new ProcessStartInfo
            {
                Arguments = GetCommandLineArguments(),
                FileName = "java",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            // Start the process.
            wrappedProcess = Process.Start(hubStartupInfo);

            if (useLog)
            {
                // Stream file.
                var fileInfo = new FileInfo(options.Log);
                logWatcher = new FileSystemWatcher
                {
                    NotifyFilter = NotifyFilters.LastWrite
                        | NotifyFilters.FileName,
                    Path = fileInfo.DirectoryName
                };
                logWatcher.Changed += LogWatcher_Log;
                logWatcher.Changed += LogWatcher_OutputDataRecieved;
                logWatcher.EnableRaisingEvents = true;
            }
            else
            {
                // Stream stderr/stdout.
                wrappedProcess.ErrorDataReceived += HubProcess_OutputDataReceived;
            }

            // Always listen to stdout/stderr.
            wrappedProcess.OutputDataReceived += HubProcess_Log;
            wrappedProcess.ErrorDataReceived += HubProcess_Log;
            wrappedProcess.BeginOutputReadLine();
            wrappedProcess.BeginErrorReadLine();

            // Wait for hub to start.
            signal.WaitOne(TimeSpan.FromSeconds(30));

            // Check if the register url is null.
            if (String.IsNullOrEmpty(NodeRegisterUrl?.ToString()))
                throw new Exception("Failed to set the node register url.");

            // Remove unecessary event listeners.
            if (useLog)
                logWatcher.Changed -= LogWatcher_OutputDataRecieved;
            else
                wrappedProcess.ErrorDataReceived -= HubProcess_OutputDataReceived;

            if (wrappedProcess.HasExited)
            {
                Dispose();
                throw new Exception("Failed to start the hub process.");
            }
        }

        /// <summary>
        /// Creates the command line arguments.
        /// </summary>
        /// <returns></returns>
        protected override string GetCommandLineArguments()
        {
            var sb = new StringBuilder();

            AddCommand(sb, "jar", options.JarFileName);
            AddCommand(sb, "role", "hub");
            AddCommand(sb, opts => opts.Log);
            AddCommand(sb, opts => opts.Host);
            AddCommand(sb, opts => opts.PortNumber);

            foreach (var servlet in options.Servlets)
                AddCommand(sb, "servlets", servlet);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <returns></returns>
        public override IList<SeleniumLogEntry> GetLogs()
        {
            return hubLogs
                .Select(l => SeleniumLogEntry.ParseString(l))
                .ToList();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Stop();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SeleniumHub() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.        
        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #endregion
    }
}
