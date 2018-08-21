using System;
using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.WebElements.Inputs
{
    public class InputElement : IWebElement
    {
        #region Fields

        private readonly IWebElement Element;

        #endregion

        #region Constructor

        public InputElement(IWebElement element)
        {
            Element = element;
        }

        #endregion

        #region Properties

        public string TagName => Element.TagName;

        public string Text => Element.Text;

        public bool Enabled => Element.Enabled;

        public bool Selected => Element.Selected;

        public Point Location => Element.Location;

        public Size Size => Element.Size;

        public bool Displayed => Element.Displayed;

        #endregion

        #region Methods

        public void Clear()
        {
            Element.Clear();
        }

        public void Click()
        {
            Element.Click();
        }

        public IWebElement FindElement(By by)
        {
            return Element.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return Element.FindElements(by);
        }

        public string GetAttribute(string attributeName)
        {
            return Element.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return Element.GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            return Element.GetProperty(propertyName);
        }

        public void SendKeys(string text)
        {
            Element.SendKeys(text);
        }

        public void Submit()
        {
            Element.Submit();
        }

        public T GetValue<T>() where T:IConvertible
        {
            var value = Element.GetAttribute("value");

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public virtual void SetValue<T>(T value) where T:IConvertible
        {
            var asString = (string)Convert.ChangeType(value, typeof(string));
            Element.Clear();
            Element.SendKeys(asString);
        }

        #endregion
    }
}