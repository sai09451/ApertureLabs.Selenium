using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Represents a web-page whose url never changes.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IPageObject" />
    public interface IStaticPageObject : IPageObject
    {
        /// <summary>
        /// Tries to navigate to the IPageObjects Uri before executing
        /// <c>Load</c>. If the Uri is null and the UriMatcher contains groups
        /// this will throw an exception.
        /// </summary>
        /// <param name="firstNavigateToUrl">if set to <c>true</c> [first navigate to URL].</param>
        /// <returns></returns>
        ILoadableComponent Load(bool firstNavigateToUrl);
    }
}
