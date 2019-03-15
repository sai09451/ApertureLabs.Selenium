namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Status returns information about whether a remote end is in a state
    /// in which it can create new sessions, but may additionally include
    /// arbitrary meta information that is specific to the implementation.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/webdriver/#dfn-status for additional details.
    /// </remarks>
    public interface IHubStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IHubStatus"/> is ready.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ready; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// See https://www.w3.org/TR/webdriver/#dfn-readiness-state for
        /// additional details.
        /// </remarks>
        bool Ready { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }
    }
}
