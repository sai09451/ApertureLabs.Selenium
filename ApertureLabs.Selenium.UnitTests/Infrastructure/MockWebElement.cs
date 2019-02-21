using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ApertureLabs.Selenium.UnitTests.Infrastructure
{
    public class MockTextWebElement : IWebElement, IWrapsDriver, IJavaScriptExecutor
    {
        #region Constructor

        public MockTextWebElement(string tagName = "div",
            string text = default,
            bool enabled = true,
            bool selected = true,
            Point location = default,
            Size size = default,
            bool displayed = true,
            IDictionary<string, string> attributes = default,
            IWebDriver driver = default,
            object scriptResult = default)
        {
            TagName = tagName;
            Text = text ?? String.Empty;
            Selected = selected;
            Location = location;
            Size = size;
            Displayed = displayed;
            Attributes = attributes ?? new Dictionary<string, string>();
            WrappedDriver = driver ?? new MockWebDriver(
                elements: new[] { this },
                scriptResult: scriptResult);
        }

        #endregion

        #region Properties

        private object ScriptResult { get; set; }

        private IDictionary<string, string> Attributes { get; set; }

        public string TagName { get; private set; }

        public string Text { get; private set; }

        public bool Enabled { get; private set; }

        public bool Selected { get; private set; }

        public Point Location { get; private set; }

        public Size Size { get; private set; }

        public bool Displayed { get; private set; }

        public IWebDriver WrappedDriver { get; private set; }

        #endregion

        #region Methods

        public void Clear()
        { }

        public void Click()
        { }

        public IWebElement FindElement(By by)
        {
            throw new NoSuchElementException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return new List<IWebElement>().AsReadOnly();
        }

        public string GetAttribute(string attributeName)
        {
            return Attributes[attributeName];
        }

        public string GetCssValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public string GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public void SendKeys(string text)
        { }

        public void Submit()
        { }

        object IJavaScriptExecutor.ExecuteScript(string script, params object[] args)
        {
            return ScriptResult;
        }

        object IJavaScriptExecutor.ExecuteAsyncScript(string script, params object[] args)
        {
            return ScriptResult;
        }

        #endregion
    }
}
