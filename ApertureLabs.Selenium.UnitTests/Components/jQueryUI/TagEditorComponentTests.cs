using ApertureLabs.Selenium.Components.JQuery.TagEditor;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.UnitTests.Infrastructure;
using ApertureLabs.Selenium.UnitTests.TestAttributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockServer.PageObjects;
using MockServer.PageObjects.Home;
using MockServer.PageObjects.Widget;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.UnitTests.Components.jQueryUI
{
    [TestClass]
    public class TagEditorComponentTests
    {
        #region Fields

        private static WebDriverFactory webDriverFactory;
        private static TagEditorComponent<WidgetPage> tagEditor;
        private static IPageObjectFactory pageObjectFactory;
        private static IWebDriver driver;
        private static WidgetPage widgetPage;

        public TestContext TestContext { get; set; }

        #endregion

        #region Setup/Cleanup

        [ClassInitialize]
        public static void Setup(TestContext testContext)
        {
            webDriverFactory = new WebDriverFactory();

            driver = webDriverFactory.CreateDriver(
                MajorWebDriver.Chrome,
                WindowSize.DefaultDesktop);

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(driver)
                .AddSingleton(new PageOptions
                {
                    Url = Startup.ServerUrl
                });

            pageObjectFactory = new PageObjectFactory(serviceCollection);

            widgetPage = pageObjectFactory.PreparePage<HomePage>()
                .GoToWidget(
                    "jQueryUI",
                    "1.12",
                    "TagEditor");

            tagEditor = pageObjectFactory.PrepareComponent(
                new TagEditorComponent<WidgetPage>(
                    By.CssSelector("#tageditor"),
                    driver,
                    widgetPage,
                    new TagEditorConfiguration()));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            webDriverFactory.Dispose();
        }

        [TestInitialize]
        public void TestStartup()
        {
            var selectedTags = tagEditor.GetSelectedTags();

            foreach (var tag in selectedTags)
                tagEditor.DeselectTag(tag);
        }

        #endregion

        #region Tests

        [Description("Verifing the ctor doesn't throw any errors. Can " +
            "ignore this test")]
        [TestMethod]
        [ServerRequired]
        public void TagEditorTest()
        { }

        [TestMethod]
        [ServerRequired]
        public void GetAllAutoCompleteOptionsTest()
        {
            var allTags = tagEditor.GetAllAutoCompleteOptions().ToArray();

            CollectionAssert.AreEqual(
                allTags,
                new[]
                {
                    "Apple",
                    "Banana",
                    "Orange",
                    "Pineapple",
                    "Kiwi",
                    "Star-fruit",
                    "Tangerine",
                    "Mango"
                });
        }

        [TestMethod]
        [ServerRequired]
        public void GetSelectedTagsTest()
        {
            var expectedResults = new[] { "Apple", "Test", "Orange" };

            foreach (var tag in expectedResults)
                tagEditor.SelectTag(tag);

            var selectedTags = tagEditor.GetSelectedTags().ToArray();

            CollectionAssert.AreEqual(selectedTags, expectedResults);
        }

        [DataTestMethod]
        [DataRow(new[] { "Mango", "Apple", "Orange" }, "Apple", true)]
        [DataRow(new[] { "Mango", "Apple", "Orange" }, "Test", false)]
        [ServerRequired]
        public void IsTagSelectedTest(IEnumerable<string> enterTagNames,
            string checkForTagName,
            bool expectedResult)
        {
            foreach (var tag in enterTagNames)
                tagEditor.SelectTag(tag);

            var isSelected = tagEditor.IsTagSelected(checkForTagName);

            Assert.AreEqual(isSelected, expectedResult);
        }

        [DataTestMethod]
        [DataRow("Mango")]
        [DataRow("Tag with space in it")]
        [DataRow("")] // Empty tag name.
        [ServerRequired]
        public void SelectTagTest(string tagName)
        {
            bool isSelected = false;

            if (String.IsNullOrEmpty(tagName))
            {
                try
                {
                    tagEditor.SelectTag(tagName);
                }
                catch (ArgumentException)
                {
                    // Test passed.
                    isSelected = true;
                }
            }
            else
            {
                tagEditor.SelectTag(tagName);
                isSelected = tagEditor.IsTagSelected(tagName);
            }

            Assert.IsTrue(isSelected);
        }

        [DataTestMethod]
        [DataRow("Apple")]
        [DataRow("Testing 1 2 3")]
        [DataRow("1 2 3 other test")]
        [ServerRequired]
        public void DeselectTagTest(string tagName)
        {
            var stillSelected = tagEditor
                .SelectTag(tagName)
                .DeselectTag(tagName)
                .IsTagSelected(tagName);

            Assert.IsFalse(stillSelected);
        }

        #endregion
    }
}
