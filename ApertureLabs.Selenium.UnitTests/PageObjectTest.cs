using ApertureLabs.Selenium.PageObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Drawing;

namespace ApertureLabs.Selenium.UnitTests
{
    public class DemoPage : PageObject
    {
        public DemoPage(IWebDriver driver) : base(driver)
        { }

        public override ILoadableComponent Load()
        {
            return base.Load();
        }
    }

    [TestClass]
    public class PageObjectTest
    {
        [TestMethod]
        public void Load()
        {
            var factory = new WebDriverFactory();
            var driver = factory.CreateDriver(MajorWebDriver.Chrome,
                new Size(1000, 1001));

            using (driver)
            {
                var page = new DemoPage(driver);
                page.Load();
            }
        }
    }
}
