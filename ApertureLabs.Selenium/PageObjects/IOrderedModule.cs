using Autofac.Core;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Similary to IModule but adds an index property to order the imports
    /// modules by. NOTE: Classes that implement this MUST have a default
    /// constructor (no-args) otherwise they will be ignored.
    /// </summary>
    public interface IOrderedModule : IModule
    {
        /// <summary>
        /// Gets the order. Lower numbers will be imported earlier.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        int Order { get; }
    }
}
