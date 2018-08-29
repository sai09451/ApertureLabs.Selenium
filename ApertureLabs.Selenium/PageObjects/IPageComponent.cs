using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.PageObjects
{
    public interface IPageComponent : ILoadableComponent,
        IWrapsDriver,
        IWrapsElement
    {
        /// <summary>
        /// The selector for the parent node for this component.
        /// </summary>
        By By { get; }

        /// <summary>
        /// Determines if the page is in a usable state.
        /// </summary>
        /// <returns></returns>
        bool IsStale();
    }
}
