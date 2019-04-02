using System;

namespace ApertureLabs.Selenium.CodeGeneration
{
    /// <summary>
    /// Defines how to implement the interface.
    /// </summary>
    public class ImplementInterfaceContext
    {
        /// <summary>
        /// Gets or sets the type of the interface.
        /// </summary>
        /// <value>
        /// The type of the interface.
        /// </value>
        public Type InterfaceType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [implement explicitly].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [implement explicitly]; otherwise, <c>false</c>.
        /// </value>
        public bool ImplementExplicitly { get; set; }
    }
}
