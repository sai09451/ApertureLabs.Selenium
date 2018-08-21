using Aperture.NopPageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;

namespace NopPageObjects.Common
{
    public class ContactUsPage : BasePage<string>
    {
        #region Fields

        private const string enquiryInputSelector = "";

        [FindsBy(How = How.CssSelector, Using = ".fullname")]
        private IWebElement nameInputElement;

        [FindsBy(How = How.CssSelector, Using = ".email")]
        private IWebElement emailInputElement;

        [FindsBy(How = How.CssSelector, Using = ".enquiry")]
        private IWebElement enquiryInputElement;

        #endregion

        #region Constructor

        public ContactUsPage(IWebDriver driver) : base(driver)
        {
            base.InitElements();
        }

        #endregion

        #region Properties

        public string Name {
            get
            {
                return nameInputElement.GetAttribute("value");
            }
            set
            {

            }
        }

        public string Email { get; set; }

        public string Enquiry { get; set; }

        #endregion

        #region Methods

        public ContactUsPage Submit()
        {
            const string submitBttnSelector = "[name='send-email']";

            Driver.FindElement(By.CssSelector(submitBttnSelector));

            return this;
        }

        #endregion
    }
}