namespace ApertureLabs.Selenium.Components
{
    /// <summary>
    /// Used to declare a modal component is present. Because of how many
    /// conditions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasModal<T> where T : IModalWindow
    {
        /// <summary>
        /// Determines whether the modal is open.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if the modal is open; otherwise, <c>false</c>.
        /// </returns>
        bool IsModalOpen();

        /// <summary>
        /// Gets the modal.
        /// </summary>
        /// <returns></returns>
        T GetModal();
    }
}
