﻿using System;
using ApertureLabs.Selenium.Components.JQuery.DatePicker;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.UnitTests.Components.jQueryUI
{
    [TestClass]
    public class DatePickerComponentTests
    {
        #region Fields

        private static WebDriverFactory WebDriverFactory;

        private IPageObjectFactory pageObjectFactory;
        private IWebDriver driver;
        private DatePickerComponent datepickerInline;
        private DatePickerComponent datepickerPopUp;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Teardown

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            WebDriverFactory = new WebDriverFactory();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            WebDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestStartup()
        {
            driver = WebDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(driver)
                .AddSingleton(new PageOptions
                {
                    Url = Startup.ServerUrl
                });

            pageObjectFactory = new PageObjectFactory(serviceCollection);

            pageObjectFactory.PreparePage<HomePage>()
                .GoToWidget(
                    "jQueryUI",
                    "1.12",
                    "DatePicker");

            datepickerInline = pageObjectFactory.PrepareComponent(
                new DatePickerComponent(
                    By.CssSelector("#datepicker1"),
                    DatePickerComponentOptions.Default(),
                    driver));

            datepickerPopUp = pageObjectFactory.PrepareComponent(
                new DatePickerComponent(
                    By.CssSelector("#datepicker2"),
                    DatePickerComponentOptions.Default(),
                    driver));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driver?.Dispose();
        }

        #endregion

        #region Tests

        [Description("Checks that no exceptions occur when loading the " +
            "component.")]
        [ServerRequired]
        [TestMethod]
        public void DatePickerComponentTest()
        { }

        [ServerRequired]
        [TestMethod]
        public void GetDateTest()
        {
            var firstDate = datepickerInline.GetDate();
            var secondDate = datepickerPopUp.GetDate();

            Assert.IsNotNull(firstDate);
            Assert.IsNull(secondDate);
        }

        [ServerRequired]
        [TestMethod]
        public void SetDateTest()
        {
            var date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, date.Day);
            date = date.AddMonths(2);
            date = date.AddDays(3);

            datepickerInline.SetDate(date);
            datepickerPopUp.SetDate(date);

            var firstDate = datepickerInline.GetDate();
            var secondDate = datepickerPopUp.GetDate();

            Assert.AreEqual(firstDate, date);
            Assert.AreEqual(secondDate, date);
        }

        #endregion
    }
}