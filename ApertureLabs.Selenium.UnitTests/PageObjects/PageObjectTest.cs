﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;

namespace ApertureLabs.Selenium.PageObjects.Tests
{
    public class DemoPage : StaticPageObject
    {
        public DemoPage(IWebDriver driver)
            : base(driver, new Uri("https://www.google.com"))
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
            var driver = factory.CreateDriver(MajorWebDriver.Firefox,
                new Size(1000, 1001));

            using (driver)
            {
                var page = new DemoPage(driver);
                page.Load();
            }
        }
    }
}
