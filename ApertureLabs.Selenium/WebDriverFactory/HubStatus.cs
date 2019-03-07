namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Implementation of IHubStatus.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.IHubStatus" />
    public class HubStatus : IHubStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IHubStatus" /> is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ready; otherwise, <c>false</c>.
        /// </value>
        /// <see cref="https://www.w3.org/TR/webdriver/#dfn-readiness-state" />
        public bool Ready { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}
