using System.Collections.Generic;

namespace ApertureLabs.Selenium
{
    public class SeleniumHubOptions
    {
        public bool? AlwaysCreateHub { get; set; }
        public int? PortNumber { get; set; }
        public bool? ShutDownHubIfRemote { get; set; }
        public bool? UseLocalHubIfAlreadyRunning { get; set; }
        public IEnumerable<string> Servlets { get; set; }

        /// <summary>
        /// Defaults this instance.
        /// </summary>
        /// <returns></returns>
        public static SeleniumHubOptions Default()
        {
            return new SeleniumHubOptions
            {
                AlwaysCreateHub = false,
                PortNumber = 4444,
                UseLocalHubIfAlreadyRunning = true,
                Servlets = new[] { "org.openqa.grid.web.servlet.LifecycleServlet" }
            };
        }
    }
}
