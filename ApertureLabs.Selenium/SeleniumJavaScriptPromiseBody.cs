using System;
using System.Threading.Tasks;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Statuses for javascript promises.
    /// </summary>
    public enum SeleniumJavaScriptPromiseStatus
    {
        /// <summary>
        /// The promise never finished.
        /// </summary>
        NotFinished = 0,

        /// <summary>
        /// The promise resolved successfully.
        /// </summary>
        Resolved = 1,

        /// <summary>
        /// The promise was rejected.
        /// </summary>
        Rejected = 2
    };

    /// <summary>
    /// A helper class for creating a javascript promise body for selenium.
    /// </summary>
    public class SeleniumJavaScriptPromiseBody
    {
        private const string FullPromiseObjectName = "window.Aperture.Selenium.Promises";

        private const string CreatePromiseObjectScript =
            "if (window.Aperture == null) {" +
                "window.Aperture = { Selenium: { Promises: [] } };" +
            "} else if (window.Aperture.Selenium == null) {" +
                "window.Aperture.Selenium = { Promises: [] };" +
            "} else if (window.Aperture.Selenium.Promises == null) {" +
                "window.Aperture.Selenium.Promises = [];" +
            "}";

        private const string CustomResolverScript = "customResolver";
        private const string CustomRejectorScript = "customRejector";

        private int promiseId;
        private string createdScript;
        private IWebDriver driver;
        private IWebElement element;

        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        public SeleniumJavaScriptPromiseBody()
        {
            promiseId = -1;
        }

        /// <summary>
        /// Ctor. Sets the PromiseBody.
        /// </summary>
        /// <param name="promiseBody"></param>
        public SeleniumJavaScriptPromiseBody(string promiseBody) : this()
        {
            PromiseBody = promiseBody;
        }

        #endregion

        #region Properties

        private string PromiseStatusScript =>
            "var callback = arguments[arguments.length - 1];" +
            $"var promise = {FullPromiseObjectName}[{PromiseId}].promise;" +
            "promise.then(function () {" +
                "callback(1);" +
            "}).catch(function () {" +
                "callback(2);" +
            "});" +
            "setTimeout(function () {" +
                "callback(0);" +
            "}, 10);";

        /// <summary>
        /// Used to determine if the promise was created.
        /// </summary>
        protected virtual bool Created => PromiseId > -1;

        /// <summary>
        /// The wrapper around the script.
        /// </summary>
        protected virtual string PromiseWrapper
        {
            get
            {
                return
                    "var {2} = arguments;" +
                    "var wrappedPromise = {{" +
                        $"{CustomResolverScript}: null," +
                        $"{CustomRejectorScript}: null," +
                        "promise: null" +
                    "}};" +
                    "var promise = new Promise(function({0}, {1}) {{" +
                        "wrappedPromise." + CustomResolverScript + " = {0};" +
                        "wrappedPromise." + CustomRejectorScript + " = {1};" +
                        "{3}" +
                    "}});" +
                    "wrappedPromise.promise = promise;" +
                    $"return {FullPromiseObjectName}.push(wrappedPromise) - 1;";
            }
        }

        /// <summary>
        /// Id of the created promise. Returns -1 if the promise hasn't been
        /// 'created' yet.
        /// </summary>
        protected virtual int PromiseId => promiseId;

        /// <summary>
        /// The body of the promise. Supports the following tokens:
        /// {0} - To resolve the promise (need to call with paranthesis IE:
        ///     {0}(23) will resolve 23).
        /// {1} - To reject the promise need to call with paranthesis IE:
        ///     {1}(43) will reject and pass the value 43).
        /// {2} - To refer to the arguments array of the intial function.
        ///     * Attempting to reference the arguments object by name in the
        ///         script will return the promises arguments and not what was
        ///         passed in.
        /// </summary>
        public virtual string PromiseBody { get; set; }

        /// <summary>
        /// Name of the function to resolve the promise.
        /// </summary>
        protected virtual string ResolveScript => "resolve";

        /// <summary>
        /// Name of the function to reject the script.
        /// </summary>
        protected virtual string RejectScript => "reject";

        /// <summary>
        /// Name of the variable the initial arguments are assigned to. Since
        /// any parameters passed into the script will only be assigned to the
        /// initial arguments object, a variable is created to reference the
        /// intial arguments to access it from the Promise.
        /// </summary>
        protected virtual string ArgumentsScript => "args";

        /// <summary>
        /// Checks if the promise was resolved.
        /// </summary>
        public virtual bool Resolved
        {
            get
            {
                if (!Created)
                    throw new Exception("Never created the promise.");

                return GetPromiseStatus() == SeleniumJavaScriptPromiseStatus.Resolved;
            }
        }

        /// <summary>
        /// Checks if the promise was rejected.
        /// </summary>
        public virtual bool Rejected
        {
            get
            {
                if (!Created)
                    throw new Exception("Never created the promise.");

                return GetPromiseStatus() == SeleniumJavaScriptPromiseStatus.Rejected;
            }
        }

        /// <summary>
        /// Checks if the promise was resolved or rejected.
        /// </summary>
        public virtual bool Finished
        {
            get
            {
                if (!Created)
                    throw new Exception("Never created the promise.");

                return GetPromiseStatus() != SeleniumJavaScriptPromiseStatus.NotFinished;
            }
        }

        /// <summary>
        /// Checks if the context the promise was saved in is still valid. IE:
        /// navigating off the page will make this a stale promise.
        /// </summary>
        public virtual bool IsStale => element.IsStale();

        #endregion

        /// <summary>
        /// This will create the js promise in the drivers active window and
        /// sets the PromiseId. Can only call this method once. Will throw an
        /// exception if the PromiseBody was never set.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="args"></param>
        public virtual void CreateScript(IWebDriver driver, params object[] args)
        {
            // Verify this wasn't called more than once.
            if (Created)
                throw new Exception("Can only call 'CreateScript' once.");
            else if (String.IsNullOrEmpty(PromiseBody))
                throw new Exception(nameof(PromiseBody));

            // Store the driver and a reference to the page to check if the
            // promise isn't stale.
            this.driver = driver;
            element = driver.FindElement(By.CssSelector("*"));

            // Verify the promises array exists.
            AssertJavaScriptPromiseArrayExists();

            var bodyScript = String.Format(PromiseBody,
                ResolveScript,
                RejectScript,
                ArgumentsScript);

            createdScript = String.Format(PromiseWrapper,
                ResolveScript,
                RejectScript,
                ArgumentsScript,
                bodyScript);

            var response = driver.JavaScriptExecutor()
                .ExecuteScript(createdScript, args)
                .ToString();

            promiseId = int.Parse(response);
        }

        /// <summary>
        /// Attempts to reject the promise with a value.
        /// </summary>
        public void Reject<T>(T rejectWith)
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomRejectorScript}({rejectWith.ToString()});";

            driver.ExecuteJavaScript(script, rejectWith);
        }

        /// <summary>
        /// Attempts to reject the promise.
        /// </summary>
        public void Reject()
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomRejectorScript}();";

            driver.ExecuteJavaScript(script);
        }

        /// <summary>
        /// Attempts to resolve the promise with a value.
        /// </summary>
        public void Resolve<T>(T resolveWith)
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomResolverScript}({resolveWith.ToString()});";

            driver.ExecuteJavaScript(script, resolveWith);
        }

        /// <summary>
        /// Attempts to resolve the promise.
        /// </summary>
        public void Resolve()
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomResolverScript}();";

            driver.ExecuteJavaScript(script);
        }

        /// <summary>
        /// Returns the status of the promise. Throws an exception if the
        /// promise was never created.
        /// </summary>
        /// <returns></returns>
        public SeleniumJavaScriptPromiseStatus GetPromiseStatus()
        {
            if (!Created)
                throw new Exception("Promise was never created.");

            var result = driver.JavaScriptExecutor()
                .ExecuteAsyncScript(PromiseStatusScript)
                .ToString();

            var asInt = int.Parse(result);

            return (SeleniumJavaScriptPromiseStatus)asInt;
        }

        /// <summary>
        /// Waits for the promise to either resolve or reject.
        /// </summary>
        /// <param name="wait">Defaults to thirty seconds.</param>
        /// <returns></returns>
        public SeleniumJavaScriptPromiseStatus Wait(TimeSpan? wait = null)
        {
            if (!Created)
                throw new Exception("Never created the script.");

            var task = Task.Run(() =>
            {
                try
                {
                    driver.Wait(wait ?? TimeSpan.FromSeconds(30))
                        .Until(d => Finished);
                }
                catch (WebDriverTimeoutException)
                { }

                return GetPromiseStatus();
            });

            task.Wait();
            return task.Result;
        }

        private void AssertJavaScriptPromiseArrayExists()
        {
            // Verify there is a (hopefully) unique location to store the
            // promises.
            driver.ExecuteJavaScript(CreatePromiseObjectScript);
        }

        /// <summary>
        /// Calls the ctor with the string as the argument.
        /// </summary>
        /// <param name="script"></param>
        public static implicit operator SeleniumJavaScriptPromiseBody(string script)
        {
            return new SeleniumJavaScriptPromiseBody(script);
        }
    }
}
