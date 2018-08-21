using OpenQA.Selenium;

namespace ApertureLabs.Selenium.WebDriver
{
    public class JavascriptHelper
    {
        #region Fields

        private readonly WebDriverV2 driver;

        #endregion

        #region Constructor

        public JavascriptHelper(WebDriverV2 driver)
        {
            this.driver = driver;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Detects if jQuery is defined on a page.
        /// </summary>
        public bool PageHasJQuery
        {
            get
            {
                var script = "(function() { return jQuery == null })()";
                return ExecuteJs<bool>(script);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Runs a script on the webpage, waits for it to finish and returns
        /// a value from it.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        public T ExecuteJs<T>(string script, IWebElement element = null)
        {
            return (T)(((IJavaScriptExecutor)driver.GetNativeWebDriver())
                .ExecuteScript(script, element));
        }

        /// <summary>
        /// Runs a script on the webpage and waits for it to finish.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        public void ExecuteJs(string script, IWebElement element = null)
        {
            ((IJavaScriptExecutor)driver.GetNativeWebDriver())
                .ExecuteScript(script, element);
        }

        /// <summary>
        /// Executes a click using jQuery.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public void JQueryClick(string selector)
        {
            ExecuteJs($"$(\"{selector}\").click()");
        }
        #endregion
    }
}
