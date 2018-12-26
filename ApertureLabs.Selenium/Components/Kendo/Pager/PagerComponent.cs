using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace ApertureLabs.Selenium.Components.Kendo.Pager
{
    /// <summary>
    /// PagerComponent.
    /// </summary>
    public class PagerComponent : BaseKendoComponent
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
        public PagerComponent(IWebDriver driver, By selector)
            : base(driver, selector)
        { }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Sets the page the listed page with the matching number.
        /// </summary>
        /// <param name="listedPage"></param>
        /// <returns></returns>
        public PagerComponent SetPage(int listedPage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of listed pages.
        /// </summary>
        /// <returns></returns>
        public IList<int> GetListedPages()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Goes to the last page if available.
        /// </summary>
        /// <returns></returns>
        public PagerComponent LastPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Goes to the first page if available.
        /// </summary>
        /// <returns></returns>
        public PagerComponent FirstPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Goes to the previous page if available.
        /// </summary>
        /// <returns></returns>
        public PagerComponent PrevPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Goes to the next page if available.
        /// </summary>
        /// <returns></returns>
        public PagerComponent NextPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the total items.
        /// </summary>
        /// <returns></returns>
        public int GetTotalItems()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the available items per page.
        /// </summary>
        /// <returns></returns>
        public IList<int> GetAvailableItemsPerPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ret
        /// </summary>
        /// <returns></returns>
        public int GetItemsPerPage()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the 'Items per Page'.
        /// </summary>
        /// <param name="itemsPerPage"></param>
        public void SetItemsPerPage(int itemsPerPage)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Refreshes the context the pager is attached to.
        /// </summary>
        public void Refresh()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
