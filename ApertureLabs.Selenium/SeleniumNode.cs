using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for creating selenium nodes.
    /// </summary>
    public class SeleniumNode : IDisposable
    {
        #region Fields

        private readonly Process nodeProcess;
        private readonly SeleniumNodeOptions seleniumNodeOptions;

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
        {
            seleniumNodeOptions = seleniumNodeOptions
                ?? throw new ArgumentNullException(nameof(seleniumNodeOptions));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the node process.
        /// </summary>
        public void Start()
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = GetCommandLineArguments(),
            };
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {

        }

        /// <summary>
        /// Determines whether this instance is running.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRunning()
        {
            return nodeProcess?.HasExited ?? false;
        }

        private string GetCommandLineArguments()
        {
            var sb = new StringBuilder("java -jar ");

            AddCommand(sb,
                opts => opts.Capabilities,
                result =>
                {
                    var capabilitiesList = new List<string>();

                    foreach (var capability in seleniumNodeOptions.Capabilities)
                        capabilitiesList.Add($"{capability.Key}={capability.Value}");

                    return String.Join(",", capabilitiesList);
                });
            AddCommand(sb, opts => opts.BrowserTimeout);
            AddCommand(sb, opts => opts.CleanUpCycle);
            AddCommand(sb, opts => opts.DownPollingLimit);
            AddCommand(sb, opts => opts.EnablePlatformVerification);
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
            AddCommand(sb, opts => opts.Register);
            AddCommand(sb, opts => opts.RegisterCycle);
            AddCommand(sb, opts => opts.RemoteHost);
            AddCommand(sb, opts => opts.SessionTimeout);
            AddCommand(sb, opts => opts.UnregisterIfStillDownAfter);

            return sb.ToString();
        }

        private void AddCommand<T>(StringBuilder commandLineArgs,
            Expression<Func<SeleniumNodeOptions, T>> property,
            Func<T, string> customFormatter = null)
        {
            var convertToLowerCase = typeof(bool) == typeof(T);
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
                commandName.Remove(0, 1);
                commandName.Insert(0, Char.ToLower(commandName[0]).ToString());
            }

            // Command value.
            var result = property.Compile().Invoke(seleniumNodeOptions);

            // Ignore if result is null.
            if (result == null)
                return;

            // Format the value.
            commandValue = customFormatter?.Invoke(result) ?? result.ToString();

            // Add whitespace if needed.
            if (commandLineArgs.Length > 0)
                commandLineArgs.Append(" ");

            commandLineArgs.Append($"-{commandName}");

            if (!String.IsNullOrEmpty(commandValue))
                commandLineArgs.Append($" {commandLineArgs}");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SeleniumNode() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
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
