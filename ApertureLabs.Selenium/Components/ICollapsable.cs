namespace ApertureLabs.Selenium.Components
{
    /// <summary>
    /// Provides methods to collapse a component. Meant to be used on a
    /// <see cref="PageObjects.PageComponent"/>.
    /// </summary>
    public interface ICollapsable
    {
        /// <summary>
        /// Collapses the component. Does nothing if already collapsed.
        /// </summary>
        void Collapse();
    }
}
