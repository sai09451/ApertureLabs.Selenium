using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Options for configuring a <c>SeleniumNode</c>.
    /// </summary>
    public class SeleniumNodeOptions
    {
        public int? BrowserTimeout { get; set; }
        public IDictionary<string, string> Capabilities { get; set; }
        public int? CleanUpCycle { get; set; }
        public int? DownPollingLimit { get; set; }
        public bool? EnablePlatformVerification { get; set; }
        public string Host { get; set; }

        /// <summary>
        /// The url that will be used to post the registration request. This
        /// option takes precedence over -hubHost and -hubPort options.
        /// </summary>
        public string Hub { get; set; }

        /// <summary>
        /// IP or hostname : the host address of the hub we're attempting to
        /// register with. If -hub is specified the -hubHost is determined from
        /// it.
        /// </summary>
        public string HubHost { get; set; }

        /// <summary>
        /// The port of the hub we're attempting to register with. If -hub is
        /// specified the -hubPort is determined from it.
        /// </summary>
        public int? HubPort { get; set; }

        public string Id { get; set; }
        public string Log { get; set; }
        public int? MaxSession { get; set; }
        public string NodeConfig { get; set; }
        public int? NodePolling { get; set; }
        public int? NodeStatusCheckTimeout { get; set; }
        public int? Port { get; set; }
        public string Proxy { get; set; }
        public bool? Register { get; set; }
        public int? RegisterCycle { get; set; }
        public string RemoteHost { get; set; }
        public int? SessionTimeout { get; set; }
        public int? UnregisterIfStillDownAfter { get; set; }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public static SeleniumNodeOptions Default()
        {
            return new SeleniumNodeOptions
            {

            };
        }
    }
}
