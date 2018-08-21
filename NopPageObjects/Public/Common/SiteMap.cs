using ApertureLabs.Selenium.PageObjects;
using Nop.Web.Models.Common;
using OpenQA.Selenium;

namespace Aperture.Nop400.PageObjects.Public.Common
{
    public class SiteMap : BasePage, IViewModel<SitemapModel>
    {
        #region Constructor

        public SiteMap(IWebDriver driver) : base(driver)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// TODO: Implement populating model from page.
        /// </summary>
        public SitemapModel ViewModel
        {
            get
            {
                return default(SitemapModel);
            }
        }

        #endregion
    }
}
