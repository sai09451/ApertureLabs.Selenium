using OpenQA.Selenium.Remote;
using System.Collections.Generic;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Options for configuring a <c>SeleniumNode</c>.
    /// </summary>
    public class SeleniumNodeOptions : SeleniumServerStandaloneOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumNodeOptions"/> class.
        /// </summary>
        public SeleniumNodeOptions()
        {
            var helper = new SeleniumServerStandAloneManager();

            if (!helper.HasLocalStandaloneJarFile())
            {
                helper.InstallVersion();
            }

            Capabilities = new[]
            {
                new Dictionary<string, string>
                {
                    { CapabilityType.BrowserName, "chrome" },
                    { "maxInstances", "5" },
                    { "seleniumProtocol", "WebDriver" }
                },
                new Dictionary<string, string>
                {
                    { CapabilityType.BrowserName, "firefox" },
                    { "maxInstances", "5" },
                    { "seleniumProtocol", "WebDriver" },
                    { "marionette", "true" }
                },
                new Dictionary<string, string>
                {
                    { CapabilityType.BrowserName, "internet_explorer" },
                    { CapabilityType.Platform, "WINDOWS" },
                    { "maxInstances", "5" },
                    { "seleniumProtocol", "WebDriver" }
                },
                new Dictionary<string, string>
                {
                    { CapabilityType.BrowserName, "edge" },
                    { CapabilityType.Platform, "WINDOWS" },
                    { "maxInstances", "5" },
                    { "seleniumProtocol", "WebDriver" }
                },
                new Dictionary<string, string>
                {
                    { CapabilityType.BrowserName, "safari" },
                    { CapabilityType.Platform, "mac" },
                    { "technologyPreview", "false" },
                    { "maxInstances", "5" },
                    { "seleniumProtocol", "WebDriver" }
                }
            };
            Hub = "http://127.0.0.1:4444/grid/register";
            JarFileName = helper.GetLocalFileNameOfVersion();
        }

        /// <summary>
        /// In seconds : number of seconds a browser session is allowed to
        /// hang while a WebDriver command is running
        /// (example: driver.get(url)). If the timeout is reached while a
        /// WebDriver command is still processing, the session will
        /// quit.Minimum value is 60. An unspecified, zero, or negative value
        /// means wait indefinitely.If a node does not specify it, the hub
        /// value will be used.
        /// </summary>
        public int? BrowserTimeout { get; set; }

        /// <summary>
        /// Comma separated Capability values.
        /// </summary>
        /// <example>
        /// Example: -capabilities
        /// browserName=firefox,platform=linux -capabilities
        /// browserName = chrome, platform = linux
        /// </example>
        /// <remarks>
        /// See https://github.com/SeleniumHQ/selenium/blob/a61cc22fd053ea827cf39aa4065ae4912bb6af2a/java/server/src/org/openqa/grid/common/defaults/DefaultNodeWebDriver.json
        /// for more details.
        /// </remarks>
        public IEnumerable<IDictionary<string, string>> Capabilities { get; set; }

        /// <summary>
        /// In ms : specifies how often the hub will poll running proxies
        /// for timed-out (i.e.hung) threads.Must also specify "timeout" option
        /// </summary>
        public int? CleanUpCycle { get; set; }

        /// <summary>
        /// Node is marked as "down" if the node hasn't responded after the
        /// number of checks specified in [downPollingLimit].
        /// </summary>
        public int? DownPollingLimit { get; set; }

        /// <summary>
        /// Whether or not to drop capabilities that does not belong to the
        /// current platform family.Defaults to true.
        /// </summary>
        public bool? EnablePlatformVerification { get; set; }

        /// <summary>
        /// IP or hostname : usually determined automatically. Most commonly
        /// useful in exotic network configurations(e.g.network with VPN)
        /// </summary>
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

        /// <summary>
        /// Optional unique identifier of the remoteHost, when not specified.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Max number of tests that can run at the same time on the node,
        /// irrespective of the browser used
        /// </summary>
        public int? MaxSession { get; set; }

        /// <summary>
        /// Filename : JSON configuration file for the node. Overrides default
        /// values
        /// </summary>
        public string NodeConfig { get; set; }

        /// <summary>
        /// In ms : specifies how often the hub will poll to see if the node is
        /// still responding.
        /// </summary>
        public int? NodePolling { get; set; }

        /// <summary>
        /// In ms : connection/socket timeout, used for node "nodePolling"
        /// check.
        /// </summary>
        public int? NodeStatusCheckTimeout { get; set; }

        /// <summary>
        /// The port number the server will use.
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// The class used to represent the node proxy. Default is
        /// [org.openqa.grid.selenium.proxy.DefaultRemoteProxy].
        /// </summary>
        public string Proxy { get; set; }

        /// <summary>
        /// If specified, node will attempt to re-register itself automatically
        /// with its known grid hub if the hub becomes unavailable.
        /// </summary>
        public bool? Register { get; set; }

        /// <summary>
        /// In ms : specifies how often the node will try to register itself
        /// again. Allows administrator to restart the hub without restarting
        /// (or risk orphaning) registered nodes. Must be specified with the
        /// "-register" option.
        /// </summary>
        public int? RegisterCycle { get; set; }

        /// <summary>
        /// URL: Address to report to the hub. Used to override default
        /// (http://&lt;host&gt;:&lt;port&gt;).
        /// </summary>
        public string RemoteHost { get; set; }

        /// <summary>
        /// In seconds : Specifies the timeout before the server automatically
        /// kills a session that hasn't had any activity in the last X seconds.
        /// The test slot will then be released for another test to use. This
        /// is typically used to take care of client crashes. For grid hub/node
        /// roles, cleanUpCycle must also be set. If a node does not specify
        /// it, the hub value will be used.
        /// </summary>
        public int? SessionTimeout { get; set; }

        /// <summary>
        /// In ms : if the node remains down for more than
        /// [unregisterIfStillDownAfter] ms, it will stop attempting to
        /// re-register from the hub.
        /// </summary>
        public int? UnregisterIfStillDownAfter { get; set; }
    }
}
