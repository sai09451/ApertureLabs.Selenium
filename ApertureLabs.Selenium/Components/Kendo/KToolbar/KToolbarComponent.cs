using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.Kendo.KToolbar
{
    /// <summary>
    /// KToolbarComponent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KToolbarComponent<T> : BaseKendoComponent<T>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KToolbarComponent{T}"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="selector"></param>
        /// <param name="driver"></param>
        /// <param name="parent">The parent.</param>
        public KToolbarComponent(BaseKendoConfiguration configuration,
            By selector,
            IWebDriver driver,
            T parent)
            : base(configuration,
                  selector,
                  driver,
                  parent)
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
