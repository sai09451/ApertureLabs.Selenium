using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Base class for the <c>SeleniumHub</c> and <c>SeleniumNode</c> classes.
    /// </summary>
    public abstract class SeleniumServerStandaloneWrapper<U> : IDisposable
        where U : SeleniumServerStandaloneOptions
    {
        #region Fields

        /// <summary>
        /// The options.
        /// </summary>
        protected readonly U options;

        /// <summary>
        /// The wrapped process.
        /// </summary>
        protected Process wrappedProcess;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumServerStandaloneWrapper{U}"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public SeleniumServerStandaloneWrapper(U options)
        {
            this.options = options
                ?? throw new ArgumentNullException(nameof(options));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        public virtual U GetOptions()
        {
            return options;
        }

        /// <summary>
        /// Determines whether this instance is running.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsRunning()
        {
            return !wrappedProcess?.HasExited ?? false;
        }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        /// <returns></returns>
        public abstract IList<SeleniumLogEntry> GetLogs();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Creates the command line arguments.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetCommandLineArguments();

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandLineArgs">The command line arguments.</param>
        /// <param name="property">The property.</param>
        /// <param name="customFormatter">The custom formatter.</param>
        /// <exception cref="Exception"></exception>
        protected void AddCommand<T>(StringBuilder commandLineArgs,
            Expression<Func<U, T>> property,
            Func<T, string> customFormatter = null)
        {
            var commandName = default(string);
            var commandValue = default(string);

            // Command name.
            if (property.Body is MemberExpression memberExpression)
            {
                commandName = memberExpression.Member.Name;
            }
            else if (property.Body is UnaryExpression unaryExpression)
            {
                var memberExp = unaryExpression.Operand as MemberExpression;
                commandName = memberExp.Member.Name;
            }
            else
            {
                throw new Exception();
            }

            // Convert to camel case if needed.
            if (Char.IsUpper(commandName[0]))
            {
                var removedLetter = commandName[0];
                commandName = commandName.Remove(0, 1);
                commandName = commandName.Insert(0, Char.ToLower(removedLetter).ToString());
            }

            // Command value.
            var result = property.Compile().Invoke(options);

            // Ignore if result is null.
            if (result == null)
                return;

            // Format the value and add it to the commandLineArgs.
            commandValue = customFormatter?.Invoke(result) ?? result.ToString();

            AddCommand(commandLineArgs, commandName, commandValue);
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="commandLineArgs">The command line arguments.</param>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandValue">The command value.</param>
        protected void AddCommand(StringBuilder commandLineArgs,
            string commandName,
            string commandValue = null)
        {
            // Add whitespace if needed.
            if (commandLineArgs.Length > 0)
                commandLineArgs.Append(" ");

            commandLineArgs.Append($"-{commandName}");

            if (!String.IsNullOrEmpty(commandValue))
                commandLineArgs.Append($" {commandValue}");
        }

        /// <summary>
        /// Determines whether the local port has a tcp listener. If the port
        /// less than or equal to zero false is returned.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        ///   <c>true</c> if [is local port busy] [the specified port]; otherwise, <c>false</c>.
        /// </returns>
        /// <see cref="https://stackoverflow.com/a/38258154"/>
        protected bool IsLocalPortBusy(int port, TimeSpan timeout)
        {
            if (port <= 0)
                return false;

            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(
                        host: "127.0.0.1",
                        port: port,
                        requestCallback: null,
                        state: null);

                    var success = result.AsyncWaitHandle.WaitOne(timeout);

                    if (!success)
                        return false;

                    client.EndConnect(result);
                }

            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the free port.
        /// </summary>
        /// <returns></returns>
        /// <see cref="https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.localendpoint?view=netstandard-2.0#remarks"/>
        /// <see cref="https://stackoverflow.com/a/150974"/>
        protected int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var endPoint = (IPEndPoint)listener.LocalEndpoint;
            var port = endPoint.Port;
            listener.Stop();

            return port;
        }

        /// <summary>
        /// Extracts the port. Returns null if the url is null or doesn't
        /// contain a port.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        protected int? ExtractPort(string url)
        {
            if (String.IsNullOrEmpty(url))
                return null;

            try
            {
                return new Uri(url).Port;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the last line of a file.
        /// </summary>
        /// <returns></returns>
        protected string GetLastLine()
        {
            var fileInfo = new FileInfo(options.Log);
            var fs = WaitForFile(
                fileInfo.FullName,
                TimeSpan.FromMilliseconds(50),
                TimeSpan.FromMilliseconds(500));
            var line = File.ReadLines(fileInfo.FullName).Last();
            fs.Dispose();

            return line;
        }

        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="poll"></param>
        /// <param name="timeout"></param>
        /// <exception cref="TimeoutException"></exception>
        /// <see cref="https://stackoverflow.com/a/50800"/>
        private FileStream WaitForFile(string fullPath,
            TimeSpan poll,
            TimeSpan timeout)
        {
            var endTime = DateTime.Now + timeout;
            var fs = default(FileStream);

            while (DateTime.Now < endTime)
            {
                try
                {
                    fs = new FileStream(
                        path: fullPath,
                        mode: FileMode.Open,
                        access: FileAccess.ReadWrite,
                        share: FileShare.Read);

                    fs.ReadByte();

                    // If we got this far the file is ready
                    break;
                }
                catch (IOException)
                {
                    fs?.Dispose();

                    // Wait for the lock to be released
                    Thread.Sleep(poll);
                }
            }

            if (fs == null)
                throw new TimeoutException();

            return fs;
        }

        #endregion
    }
}
