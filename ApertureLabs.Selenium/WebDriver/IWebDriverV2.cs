using System;
using System.Collections.Generic;
using System.Drawing;
using ApertureLabs.Selenium.WebElement;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace ApertureLabs.Selenium.WebDriver
{
    public interface IWebDriverV2
    {
        TimeSpan DefaultTimeout { get; set; }
        JavascriptHelper Javascript { get; }
        TabHelper Tabs { get; }
        UrlHelper Url { get; }

        Actions CreateAction();
        void Dispose();
        IWebDriver GetNativeWebDriver();
        void RefreshPage();
        IList<IWebElementV2> Select(string cssSelector, TimeSpan? wait = null);
        void SetWindowSize(int width, int? height = null);
        void SetWindowSize(Size size);
        TResult WaitUntil<TResult>(Func<IWebDriverV2, TResult> condition, TimeSpan? wait = null);
    }
}