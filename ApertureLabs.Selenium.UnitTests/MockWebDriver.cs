using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApertureLabs.Selenium.UnitTests
{
    public class MockWebDriver : IWebDriver, IJavaScriptExecutor
    {
        #region Fields

        private string url;
        private string title;
        private string pageSource;
        private string currentWindowHandle;
        private ReadOnlyCollection<string> windowHandles;
        private readonly IEnumerable<IWebElement> elements;

        #endregion

        #region Constructor

        public MockWebDriver(string url = default,
            string title = default,
            string pageSource = default,
            string currentWindowHandle = default,
            ReadOnlyCollection<string> windowHandles = default,
            IEnumerable<IWebElement> elements = default)
        {
            Url = url;
            Title = title;
            PageSource = pageSource;
            CurrentWindowHandle = currentWindowHandle;
            WindowHandles = windowHandles;

            this.elements = elements ?? new List<IWebElement>();
        }

        #endregion

        #region Properties

        public string Url
        {
            get => url;
            set => url = value ?? String.Empty;
        }

        public string Title
        {
            get => title;
            private set => title = value ?? String.Empty;
        }

        public string PageSource
        {
            get => pageSource;
            private set => pageSource = value ?? String.Empty;
        }

        public string CurrentWindowHandle
        {
            get => currentWindowHandle;
            private set => currentWindowHandle = value ?? String.Empty;
        }

        public ReadOnlyCollection<string> WindowHandles
        {
            get => windowHandles;
            private set => windowHandles = value ?? new List<string>().AsReadOnly();
        }

        #endregion

        #region Methods

        public void Close()
        { }

        public void Dispose()
        { }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScript(string script, params object[] args)
        {
            throw new NotImplementedException();
        }

        public IWebElement FindElement(By by)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            throw new NotImplementedException();
        }

        public IOptions Manage()
        {
            throw new NotImplementedException();
        }

        public INavigation Navigate()
        {
            throw new NotImplementedException();
        }

        public void Quit()
        { }

        public ITargetLocator SwitchTo()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
