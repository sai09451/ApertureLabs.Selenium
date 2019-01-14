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
        /// <param name="configuration"></param>
        /// <param name="selector"></param>
        /// <param name="dataSourceOptions"></param>
        /// <param name="driver"></param>
        public KToolbarComponent(BaseKendoConfiguration configuration,
            By selector,
            IWebDriver driver)
            : base(configuration,
                  selector,
                  driver)
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
