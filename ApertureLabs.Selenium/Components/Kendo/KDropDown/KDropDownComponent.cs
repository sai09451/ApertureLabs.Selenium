using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    public class KDropDownComponent : BaseKendoComponent
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
        public KDropDownComponent(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        public bool IsExpanded()
        {
            throw new NotImplementedException();
        }

        public IList<string> GetItems()
        {
            throw new NotImplementedException();
        }

        public string GetSelectedItem()
        {
            throw new NotImplementedException();
        }

        public SelectElement GetSelectElement()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
