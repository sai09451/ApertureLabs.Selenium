using ApertureLabs.Selenium.Components.Kendo;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Nop
{
    /// <summary>
    /// The default options for kendo datasources used in NopCommerce.
    /// </summary>
    public class DefaultNopDatasourceOptions : DataSourceOptions
    {
        /// <summary>
        /// Sets the <c>PageLoadingIndicator</c> to '#ajaxBusy'.
        /// </summary>
        public DefaultNopDatasourceOptions()
        {
            PageLoadingSelector = By.CssSelector("#ajaxBusy");
        }
    }
}
