using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Components.Kendo.KendoGrid
{
    /// <summary>
    /// Represents a kendo grid.
    /// </summary>
    public class KendoGridComponent : PageComponent
    {
        #region Fields

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector"></param>
        public KendoGridComponent(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        #endregion
    }
}
