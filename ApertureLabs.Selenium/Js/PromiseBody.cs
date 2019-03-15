using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;

namespace ApertureLabs.Selenium.Js
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
    public class PromiseBody : JavaScript
    {
        #region Fields

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
        private readonly IWebDriver driver;
        private IJavaScriptExecutor javaScriptExecutor;
        private IWebElement element;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PromiseBody"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">driver</exception>
        public PromiseBody(IWebDriver driver)
        {
            this.driver = driver
                ?? throw new ArgumentNullException(nameof(driver));

            promiseId = -1;
            ResolveToken = "{resolve}";
            RejectToken = "{reject}";
            ArgsToken = "{args}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromiseBody"/> class.
        /// </summary>
        /// <param name="driver">The driver.</param>
        /// <param name="promiseBody">The promise body.</param>
        public PromiseBody(IWebDriver driver, string promiseBody) : this(driver)
        {
            Script = promiseBody;
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

        private string PromiseValueScript =>
            "var callback = arguments[arguments.length - 1];" +
            $"var promise = {FullPromiseObjectName}[{PromiseId}].promise;" +
            "promise.then(function (res) {" +
                "callback(res);" +
            "}).catch(function (err) {" +
                "callback(err);" +
            "});" +
            "setTimeout(function () {" +
                "callback();" +
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
        /// Gets or sets a value indicating whether this script is
        /// asynchronous. This changes how Execute() operates, if true then the
        /// script will be executed using ExecuteAsyncScript(...), if false
        /// then ExecuteScript(...).
        /// </summary>
        public override bool IsAsync
        {
            get => false;
        }

        /// <summary>
        /// Gets or sets the arguments passed into the script when executing
        /// it.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public override ICollection<JavaScriptValue> Arguments => Array.Empty<JavaScriptValue>();

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
        /// The body of the promise.
        /// Supports the following 'default' tokens:
        /// {resolve} - To resolve the promise (need to call with paranthesis IE:
        ///     {resolve}(23) will resolve 23).
        /// {reject} - To reject the promise need to call with paranthesis IE:
        ///     {reject}(43) will reject and pass the value 43).
        /// {args} - To refer to the arguments array of the intial function.
        ///     * Attempting to reference the arguments object by name in the
        ///         script will return the promises arguments and not what was
        ///         passed in.
        /// </summary>
        public override string Script
        {
            get => base.Script;
            set => base.Script = value;
        }

        /// <summary>
        /// Gets or sets the resolve token.
        /// </summary>
        public string ResolveToken { get; set; }

        /// <summary>
        /// Gets or sets the reject token.
        /// </summary>
        public string RejectToken { get; set; }

        /// <summary>
        /// Gets or sets the arguments token.
        /// </summary>
        public string ArgsToken { get; set; }

        /// <summary>
        /// Checks if the promise was resolved.
        /// </summary>
        /// <value>
        ///   <c>true</c> if resolved; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="InvalidOperationException">Never created the promise.</exception>
        public virtual bool Resolved
        {
            get
            {
                if (!Created)
                    throw new InvalidOperationException("Never created the promise.");

                return GetPromiseStatus() == SeleniumJavaScriptPromiseStatus.Resolved;
            }
        }

        /// <summary>
        /// Checks if the promise was rejected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rejected; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="InvalidOperationException">Never created the promise.</exception>
        public virtual bool Rejected
        {
            get
            {
                if (!Created)
                    throw new InvalidOperationException("Never created the promise.");

                return GetPromiseStatus() == SeleniumJavaScriptPromiseStatus.Rejected;
            }
        }

        /// <summary>
        /// Checks if the promise was resolved or rejected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if finished; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="InvalidOperationException">Never created the promise.</exception>
        public virtual bool Finished
        {
            get
            {
                if (!Created)
                    throw new InvalidOperationException("Never created the promise.");

                return GetPromiseStatus() != SeleniumJavaScriptPromiseStatus.NotFinished;
            }
        }

        /// <summary>
        /// Checks if the context the promise was saved in is still valid. IE:
        /// navigating off the page will make this a stale promise.
        /// </summary>
        public virtual bool IsStale => element.IsStale();

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to reject the promise with a value.
        /// </summary>
        public void Reject<T>(T rejectWith)
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomRejectorScript}({rejectWith.ToString()});";

            javaScriptExecutor.ExecuteScript(script, rejectWith);
        }

        /// <summary>
        /// Attempts to reject the promise.
        /// </summary>
        public void Reject()
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomRejectorScript}();";

            javaScriptExecutor.ExecuteScript(script);
        }

        /// <summary>
        /// Attempts to resolve the promise with a value.
        /// </summary>
        public void Resolve<T>(T resolveWith)
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomResolverScript}({resolveWith.ToString()});";

            javaScriptExecutor.ExecuteScript(script, resolveWith);
        }

        /// <summary>
        /// Attempts to resolve the promise.
        /// </summary>
        public void Resolve()
        {
            var script =
                $"var promise = {FullPromiseObjectName}[{PromiseId}];" +
                $"promise.{CustomResolverScript}();";

            javaScriptExecutor.ExecuteScript(script);
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

            var result = javaScriptExecutor
                .ExecuteAsyncScript(PromiseStatusScript)
                .ToString();

            var asInt = Int32.Parse(result, CultureInfo.CurrentCulture);

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

            if (javaScriptExecutor is IWebDriver driver)
            {
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
            else
            {
                throw new NotImplementedException(
                    "The IJavaScriptExecutor wasn't an IWebDriver.");
            }
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Created
                ? createdScript
                : Script;
        }

        /// <summary>
        /// This will create the js promise in the drivers active window and
        /// sets the PromiseId. Can only call this method once. Will throw an
        /// exception if the PromiseBody was never set.
        /// </summary>
        /// <param name="executor">The executor.</param>
        public override JavaScriptValue Execute(IJavaScriptExecutor executor)
        {
            // Verify this wasn't called more than once.
            if (Created)
                throw new Exception("Can only call 'CreateScript' once.");
            else if (String.IsNullOrEmpty(Script))
                throw new Exception(nameof(Script));

            javaScriptExecutor = executor;

            // Store the driver and a reference to the page to check if the
            // promise isn't stale.
            element = driver.FindElement(By.CssSelector("*"));

            // Verify the promises array exists.
            AssertJavaScriptPromiseArrayExists();

            var bodyScript = Script
                .Replace(ResolveToken, ResolveScript)
                .Replace(RejectToken, RejectScript)
                .Replace(ArgsToken, ArgumentsScript);

            createdScript = String.Format(CultureInfo.CurrentCulture,
                PromiseWrapper,
                ResolveScript,
                RejectScript,
                ArgumentsScript,
                bodyScript);

            var response = driver.JavaScriptExecutor()
                .ExecuteScript(createdScript, Arguments)
                .ToString();

            promiseId = Int32.Parse(response, CultureInfo.CurrentCulture);

            return new JavaScriptValue(response);
        }

        /// <summary>
        /// Returns the value from the promise.
        /// </summary>
        /// <returns></returns>
        public JavaScriptValue PromiseResult()
        {
            if (!Finished)
                return new JavaScriptValue(default(object));

            var val = javaScriptExecutor
                .ExecuteAsyncScript(PromiseValueScript);

            return new JavaScriptValue(val);
        }

        private void AssertJavaScriptPromiseArrayExists()
        {
            // Verify there is a (hopefully) unique location to store the
            // promises.
            javaScriptExecutor.ExecuteScript(CreatePromiseObjectScript);
        }

        #endregion
    }
}
