using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.WebElements.IFrame
{
    /// <summary>
    /// Wrapper around the IWebElement for IFrames.
    /// </summary>
    public class IFrameElement : BaseElement
    {
        #region Fields

        private readonly IWebDriver driver;

        private int enteredIFrameCount = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="driver"></param>
        /// <exception cref="UnexpectedTagNameException">
        /// Thrown if the element isn't an iframe.
        /// </exception>
        public IFrameElement(IWebElement element, IWebDriver driver)
            : base(element)
        {
            if (!String.Equals(element.TagName, "iframe", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnexpectedTagNameException("Expected the tagname to be iframe.");
            }

            this.driver = driver;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes an action in the context of the IFrame.
        /// </summary>
        /// <param name="action"></param>
        public void InFrameAction(Action action)
        {
            EnterIframe();
            action();
            ExitIframe();
        }

        /// <summary>
        /// Executes a function in the context of the IFrame.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        public T InFrameFunction<T>(Func<T> function)
        {
            EnterIframe();
            var returnVal = function();
            ExitIframe();

            return returnVal;
        }

        /// <summary>
        /// Enters the iframe only if not already entered.
        /// Equal to IWebDriver.SwitchTo().Frame(iframeElement).
        /// </summary>
        public void EnterIframe()
        {
            if (enteredIFrameCount == 0)
            {
                driver.SwitchTo().Frame(WrappedElement);
            }

            enteredIFrameCount++;
        }

        /// <summary>
        /// Exits the iframe only if previously entered.
        /// </summary>
        public void ExitIframe()
        {
            enteredIFrameCount--;

            if (enteredIFrameCount == 0)
            {
                driver.SwitchTo().ParentFrame();
            }
        }

        #endregion
    }
}
