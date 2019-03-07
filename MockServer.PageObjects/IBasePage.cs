using ApertureLabs.Selenium.PageObjects;
using MockServer.PageObjects.Home;

namespace MockServer.PageObjects
{
    public interface IBasePage : IPageObject
    {
        HomePage GoToHomePage();
    }
}