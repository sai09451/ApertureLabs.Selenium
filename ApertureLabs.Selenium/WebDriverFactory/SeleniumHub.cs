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
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        string Id { get; }
    }

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
            : this(new SeleniumHubOptions())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumHub"/> class.
        /// </summary>
        /// <param name="seleniumHubOptions">The selenium hub options.</param>
        public SeleniumHub(SeleniumHubOptions seleniumHubOptions)
            : base(seleniumHubOptions)
        {
            signal = new AutoResetEvent(false);
            hubLogs = new List<string>();
            registeredNodes = new List<SeleniumNode>();

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

            var fileInfo = new FileInfo(Options.Log);

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

            var fileInfo = new FileInfo(Options.Log);

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
            else if (WrappedProcess == null)
                throw new Exception("Hub hasn't been started yet.");

            // Assign the register url to the hub property.
            seleniumNodeOptions.Hub = NodeRegisterUrl.ToString();
            var seleniumNode = new SeleniumNode(seleniumNodeOptions);
            seleniumNode.StartProcess();
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

            seleniumNode.StopProcess();
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
        public override void StopProcess()
        {
            if (!IsRunning())
                return;

            foreach (var node in registeredNodes)
                node.StopProcess();

            registeredNodes.Clear();

            if (Options.Servlets.Contains(DefaultServletNames.LifeCycleServlet))
            {
                // Send shutdown request.
                var shutDownUri = new Uri(Uri, "lifecycle-manager?action=shutdown");

                using (var httpClient = new HttpClient())
                {
                    httpClient
                        .GetAsync(shutDownUri)
                        .Wait(TimeSpan.FromSeconds(10));
                }

                WrappedProcess.WaitForExit(10000);
            }
            else
            {
                // Terminate the hub.
                WrappedProcess.Kill();
                WrappedProcess.WaitForExit(10000);
                WrappedProcess.Dispose();
            }

            WrappedProcess = null;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="Exception">Failed to start the hub process.</exception>
        public override void StartProcess()
        {
            // Check if port is busy.
            var port = Options.PortNumber
                ?? ExtractPort(Options.Host)
                ?? 4444;

            if (IsLocalPortBusy(port, TimeSpan.FromSeconds(1)))
                throw new Exception($"Port {port} wasn't available.");

            var useLog = !String.IsNullOrEmpty(Options.Log);

            var hubStartupInfo = new ProcessStartInfo
            {
                Arguments = GetCommandLineArguments(),
                FileName = "java",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            // Start the process.
            WrappedProcess = Process.Start(hubStartupInfo);

            if (useLog)
            {
                // Stream file.
                var fileInfo = new FileInfo(Options.Log);
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
                WrappedProcess.ErrorDataReceived += HubProcess_OutputDataReceived;
            }

            // Always listen to stdout/stderr.
            WrappedProcess.OutputDataReceived += HubProcess_Log;
            WrappedProcess.ErrorDataReceived += HubProcess_Log;
            WrappedProcess.BeginOutputReadLine();
            WrappedProcess.BeginErrorReadLine();

            // Wait for hub to start.
            signal.WaitOne(TimeSpan.FromSeconds(30));

            // Check if the register url is null.
            if (String.IsNullOrEmpty(NodeRegisterUrl?.ToString()))
                throw new Exception("Failed to set the node register url.");

            // Remove unecessary event listeners.
            if (useLog)
                logWatcher.Changed -= LogWatcher_OutputDataRecieved;
            else
                WrappedProcess.ErrorDataReceived -= HubProcess_OutputDataReceived;

            if (WrappedProcess.HasExited)
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

            AddCommand(sb, "jar", Options.JarFileName);
            AddCommand(sb, "role", "hub");
            AddCommand(sb, opts => opts.Log);
            AddCommand(sb, opts => opts.Host);
            AddCommand(sb, opts => opts.PortNumber);

            foreach (var servlet in Options.Servlets)
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

        #endregion
    }
}
