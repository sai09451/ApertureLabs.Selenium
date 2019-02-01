using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for creating selenium nodes.
    /// </summary>
    public class SeleniumNode :
        SeleniumServerStandaloneWrapper<SeleniumNodeOptions>
    {
        #region Fields

        private readonly IList<string> nodeLogs;

        private AutoResetEvent signal;
        private FileSystemWatcher logWatcher;
        private Uri nodeUrl;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumNode"/> class.
        /// </summary>
        public SeleniumNode()
            : this(SeleniumNodeOptions.Default())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumNode"/> class.
        /// </summary>
        /// <param name="seleniumNodeOptions">The selenium node options.</param>
        /// <exception cref="ArgumentNullException">seleniumNodeOptions</exception>
        public SeleniumNode(SeleniumNodeOptions seleniumNodeOptions)
            : base(seleniumNodeOptions)
        {
            nodeLogs = new List<string>();
        }

        #endregion

        #region Methods

        #region Event Listeners

        private void LogWatcher_FileDataRecieved(
            object sender,
            FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            // Check if the correct file was modified.
            var fileInfo = new FileInfo(options.Log);

            if (fileInfo.FullName != e.FullPath)
                return;

            // Read & parse the last line.
            var lastLine = File.ReadLines(fileInfo.FullName).Last();
            var logEntry = SeleniumLogEntry.ParseString(lastLine);

            // Ignore exception messages.
            if (logEntry.IsException)
                return;

            // Check if the node is up and running.
            if (logEntry.Message.Contains("Selenium Server is up and running on port"))
            {
                var match = Regex.Match(
                    logEntry.Message,
                    @"(http[^\s]*)");

                var port = match.Groups[1];
                nodeUrl = new Uri($"http://localhost:{port}");
            }

            // Check if the node is ready.
            if (logEntry.Message.Contains("The node is registered to the hub and ready to use"))
            {
                signal.Set();
            }
        }

        private void LogWatcher_FileLog(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            // ReadLines uses a stream. Won't load the entire file into memory.
            // See - https://stackoverflow.com/a/11625667
            var lastLine = File.ReadLines(options.Log).Last();
            nodeLogs.Add(lastLine);
        }

        private void NodeProcess_StdOutLog(
            object sender,
            DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
                nodeLogs.Add(e.Data);
        }

        private void LocalNodeProcess_StdOutOutputDataReceived(
            object sender,
            DataReceivedEventArgs e)
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
                signal.Set();
            }
        }

        #endregion

        /// <summary>
        /// Starts the node process.
        /// </summary>
        public override void Start()
        {
            // Check if port is busy.
            var port = options.Port
                ?? ExtractPort(options.Host)
                ?? 0;

            if (IsLocalPortBusy(port, TimeSpan.FromSeconds(1)))
                throw new Exception("");

            signal = new AutoResetEvent(false);
            var useLog = !String.IsNullOrEmpty(options.Log);
            var startInfo = new ProcessStartInfo
            {
                Arguments = GetCommandLineArguments(),
                FileName = "java",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            // Start the process.
            wrappedProcess = Process.Start(startInfo);

            // Wait for file to exists.
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
                logWatcher.Changed += LogWatcher_FileLog;
                logWatcher.Changed += LogWatcher_FileDataRecieved;
                logWatcher.EnableRaisingEvents = true;
            }
            else
            {
                // Stream stderr/stdout.
                wrappedProcess.ErrorDataReceived += LocalNodeProcess_StdOutOutputDataReceived;
            }

            // Always listen to stdout/stderr.
            wrappedProcess.OutputDataReceived += NodeProcess_StdOutLog;
            wrappedProcess.ErrorDataReceived += NodeProcess_StdOutLog;
            wrappedProcess.BeginOutputReadLine();
            wrappedProcess.BeginErrorReadLine();

            // Wait for the node to start.
            signal.WaitOne(TimeSpan.FromSeconds(30));

            // Remove unecessary event listeners.
            if (useLog)
                logWatcher.Changed -= LogWatcher_FileDataRecieved;
            else
                wrappedProcess.ErrorDataReceived -= LocalNodeProcess_StdOutOutputDataReceived;

            if (wrappedProcess.HasExited)
            {
                Dispose();
                throw new Exception("Failed to start the node process.");
            }

            signal.Dispose();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public override void Stop()
        {
            // Remove event listeners.
            if (logWatcher != null)
            {
                logWatcher.Changed -= LogWatcher_FileLog;
                logWatcher.Dispose();
                logWatcher = null;
            }

            wrappedProcess?.Kill();
            wrappedProcess = null;
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override IList<SeleniumLogEntry> GetLogs()
        {
            return nodeLogs
                .Select(l => SeleniumLogEntry.ParseString(l))
                .ToList();
        }

        /// <summary>
        /// Creates the command line arguments.
        /// </summary>
        /// <returns></returns>
        protected override string GetCommandLineArguments()
        {
            var sb = new StringBuilder();

            AddCommand(sb, "jar", options.JarFileName);
            AddCommand(sb, "role", "node");

            if (options.Capabilities.Any())
            {
                foreach (var capabilityDict in options.Capabilities)
                {
                    var capabilities = new List<string>();

                    foreach (var capability in capabilityDict)
                    {
                        var keyVal = $"{capability.Key}={capability.Value}";
                        capabilities.Add(keyVal);
                    }

                    AddCommand(sb,
                        "capabilities",
                        String.Join(",", capabilities));
                }
            }

            AddCommand(sb, opts => opts.BrowserTimeout);
            AddCommand(sb, opts => opts.CleanUpCycle);
            AddCommand(sb, opts => opts.DownPollingLimit);

            AddCommand(sb,
                opts => opts.EnablePlatformVerification,
                r => r.GetValueOrDefault().ToString().ToLower());

            AddCommand(sb, opts => opts.Host);
            AddCommand(sb, opts => opts.Hub);
            AddCommand(sb, opts => opts.HubHost);
            AddCommand(sb, opts => opts.HubPort);
            AddCommand(sb, opts => opts.Id);
            AddCommand(sb, opts => opts.Log);
            AddCommand(sb, opts => opts.MaxSession);
            AddCommand(sb, opts => opts.NodeConfig);
            AddCommand(sb, opts => opts.NodePolling);
            AddCommand(sb, opts => opts.NodeStatusCheckTimeout);
            AddCommand(sb, opts => opts.Port);
            AddCommand(sb, opts => opts.Proxy);

            AddCommand(sb,
                opts => opts.Register,
                r => r ?? false ? "" : null);

            AddCommand(sb, opts => opts.RegisterCycle);
            AddCommand(sb, opts => opts.RemoteHost);
            AddCommand(sb, opts => opts.SessionTimeout);
            AddCommand(sb, opts => opts.UnregisterIfStillDownAfter);

            return sb.ToString();
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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #endregion
    }
}
