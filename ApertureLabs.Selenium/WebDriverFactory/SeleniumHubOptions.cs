using System.Collections.Generic;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Options for the selenium hub.
    /// </summary>
    public class SeleniumHubOptions : SeleniumServerStandaloneOptions
    {

        /// <summary>
        /// Gets or sets the always create hub.
        /// </summary>
        /// <value>
        /// The always create hub.
        /// </value>
        public bool? AlwaysCreateHub { get; set; }

        /// <summary>
        /// IP or hostname : usually determined automatically. Most commonly
        /// useful in exotic network configurations(e.g.network with VPN).
        /// </summary>
        /// <value>
        /// The host.
        /// </value>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        public int? PortNumber { get; set; }
        
        /// <summary>
        /// Gets or sets whether to shut down the hub if it's a remote process
        /// when the hub is disposed.
        /// </summary>
        /// <value>
        /// The shut down hub if remote.
        /// </value>
        public bool? ShutDownHubIfRemote { get; set; }

        /// <summary>
        /// Gets or sets the use local hub if already running.
        /// </summary>
        /// <value>
        /// The use local hub if already running.
        /// </value>
        public bool? UseLocalHubIfAlreadyRunning { get; set; }

        /// <summary>
        /// Gets or sets the servlets.
        /// </summary>
        /// <value>
        /// The servlets.
        /// </value>
        public IEnumerable<string> Servlets { get; set; }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public static SeleniumHubOptions Default()
        {
            var helper = new SeleniumServerStandAloneManager();

            if (!helper.HasLocalStandaloneJarFile())
                helper.InstallVersion();

            return new SeleniumHubOptions
            {
                AlwaysCreateHub = false,
                JarFileName = helper.GetLocalFileNameOfVersion(),
                PortNumber = null,
                UseLocalHubIfAlreadyRunning = true,
                Servlets = new[] { "org.openqa.grid.web.servlet.LifecycleServlet" }
            };
        }
    }
}
