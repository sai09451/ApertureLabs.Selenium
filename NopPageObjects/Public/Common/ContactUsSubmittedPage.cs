﻿using ApertureLabs.Selenium.PageObjects;
using Nop.Web.Models.Common;
using OpenQA.Selenium;

namespace Aperture.Nop400.PageObjects.Public.Common
{
    public class ContactUsSubmittedPage : BasePage, IViewModel<ContaSitemapModelctUsModel>
    {
        #region Constructor

        public ContactUsSubmittedPage(IWebDriver driver) : base(driver)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// TODO: Implement populating model from page.
        /// </summary>
        public ContactUsModel ViewModel
        {
            get
            {
                return default(ContactUsModel);
            }
        }

        #endregion
    }
}
