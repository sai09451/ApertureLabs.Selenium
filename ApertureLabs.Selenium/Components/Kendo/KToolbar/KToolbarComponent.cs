using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Kendo.KToolbar
{
    /// <summary>
    /// KToolbarComponent.
    /// </summary>
    public class KToolbarComponent : BaseKendoComponent
    {
        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        public KToolbarComponent(IWebDriver driver,
            By selector,
            DataSourceOptions dataSourceOptions)
            : base(driver,
                  selector,
                  dataSourceOptions)
        { }

        #endregion

        #region Methods

        /// <summary>
        /// Returns all item container elements.
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyList<IWebElement> GetItems()
        {
            return WrappedElement
                .Children()
                .Where(e => !e.Classes().Contains("k-seperator"))
                .ToList()
                .AsReadOnly();
        }

        #endregion
    }
}
