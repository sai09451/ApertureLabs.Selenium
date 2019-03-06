namespace ApertureLabs.Selenium.Components
{
    /// <summary>
    /// Used on <see cref="PageObjects.PageComponent"/> that can be expanded.
    /// </summary>
    public interface IExpandable
    {
        /// <summary>
        /// Expands the component if not already expanded.
        /// </summary>
        void Expand();
    }
}
