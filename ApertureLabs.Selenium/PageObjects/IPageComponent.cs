using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.PageObjects
{
    public interface IPageComponent : ILoadableComponent,
        IWrapsDriver,
        IWrapsElement
    {
        bool IsStale();
    }
}
