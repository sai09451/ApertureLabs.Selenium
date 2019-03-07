using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Kendo
{
    /// <summary>
    /// Used to configure datasource kendo components.
    /// </summary>
    public class DataSourceOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceOptions"/> class.
        /// </summary>
        /// <remarks>
        /// Initializes a new DataSourceOptions object with RemoteDataSource
        /// set to false and the page/container loading selectors set to
        /// ".k-loading-mask");
        /// </remarks>
        public DataSourceOptions()
        {
            RemoteDataSource = false;
            PageLoadingSelector = By.CssSelector(".k-loading-mask");
            ContainerLoadingSelector = By.CssSelector(".k-loading-mask");
        }

        /// <summary>
        /// Determines if data-changing operations display a loading
        /// indicator.
        /// </summary>
        public bool RemoteDataSource { get; set; }

        /// <summary>
        /// Kendo page loading indicator. Selector is relative to the document.
        /// </summary>
        public By PageLoadingSelector { get; set; }

        /// <summary>
        /// Kendo container loading indicator. Selector is relative to the
        /// document.
        /// </summary>
        public By ContainerLoadingSelector { get; set; }
    }
}
