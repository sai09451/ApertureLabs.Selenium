using Aperture.Nop400.PageObjects.Public.Models;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace Aperture.Nop400.PageObjects.Public.Common
{
    public class ContactVendorPage : BasePage, IViewModel<ContactVendorModel>
    {
        #region Constructor

        public ContactVendorPage(IWebDriver driver) : base(driver)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// TODO: Implement populating model from page.
        /// </summary>
        public ContactVendorModel ViewModel
        {
            get
            {
                return default(ContactVendorModel);
            }
        }

        #endregion
    }
}
