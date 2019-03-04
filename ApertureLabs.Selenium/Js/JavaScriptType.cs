namespace ApertureLabs.Selenium.Js
{
    /// <summary>
    /// The Selenium supported JavaScript type for passing in and returning
    /// from a <see cref="OpenQA.Selenium.IJavaScriptExecutor">.
    /// </summary>
    public enum JavaScriptType
    {
        /// <summary>
        /// A null value.
        /// </summary>
        Null = 0,

        /// <summary>
        /// A boolean. Cast to bool.
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// An array of booleans. Cast to IEnumerable{bool}.
        /// </summary>
        BooleanArray = 2,

        /// <summary>
        /// A number (Int64/long). Cast to long.
        /// </summary>
        Number = 3,

        /// <summary>
        /// A number array (Int64/long). Cast to IEnumerable{long}.
        /// </summary>
        NumberArray = 4,

        /// <summary>
        /// A string. Cast to string.
        /// </summary>
        String = 5,

        /// <summary>
        /// A string array. Cast to IEnumerable{string}.
        /// </summary>
        StringArray = 6,

        /// <summary>
        /// An element. Cast to <see cref="OpenQA.Selenium.IWebElement"/>.
        /// </summary>
        WebElement = 7,

        /// <summary>
        /// An array of web elements. Cast to IEnumerable{IWebElement}.
        /// </summary>
        WebElementArray = 8,

        /// <summary>
        /// An array consisting of multiple types or all nulls.
        /// </summary>
        MultiTypeArray = 9
    }
}