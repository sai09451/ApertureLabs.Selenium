using OpenQA.Selenium;

namespace ApertureLabs.Selenium.WebElement
{
    public interface IWebElementV2 : ICssQueryContext
    {
        IWebElement WebElement { get; }
    }
}
