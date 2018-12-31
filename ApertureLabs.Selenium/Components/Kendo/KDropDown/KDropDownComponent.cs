using ApertureLabs.Selenium.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.Kendo.KDropDown
{
    /// <summary>
    /// Kendo dropdown widget.
    /// </summary>
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

        public override ILoadableComponent Load()
        {
            base.Load();

            // Validate the dropdown element contains the correct class.
            if (!WrappedElement.Classes().Contains("k-dropdown"))
            {
                throw new InvalidElementStateException("Missing the " +
                    "k-dropdown class on the element.");
            }

            return this;
        }

        public virtual bool IsExpanded()
        {
            throw new NotImplementedException();
        }

        public virtual IList<string> GetItems()
        {
            throw new NotImplementedException();
        }

        public virtual string GetSelectedItem()
        {
            throw new NotImplementedException();
        }

        public virtual void SetSelectedItems(params string[] values)
        {

        }

        public virtual SelectElement GetSelectElement()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
