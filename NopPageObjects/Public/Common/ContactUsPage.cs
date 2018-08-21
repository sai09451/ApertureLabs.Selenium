using ApertureLabs.Selenium.PageObjects;
using Nop.Web.Models.Common;
using OpenQA.Selenium;

namespace Aperture.Nop400.PageObjects.Public.Common
{
    public class ContactUsPage : BasePage, IViewModel<ContactUsModel>
    {
        #region Fields

        #endregion

        #region Constructor

        public ContactUsPage(IWebDriver driver) : base(driver)
        { }

        #endregion

        #region Properties

        private IWebElement EmailInputElement => WrappedDriver.FindElement(
            By.CssSelector(".email"));

        private IWebElement EnquiryInputElement => WrappedDriver.FindElement(
            By.CssSelector(".enquiry"));

        private IWebElement SubmitButton => WrappedDriver.FindElement(
            By.CssSelector("[name='send-email']"));

        private IWebElement NameInputElement => WrappedDriver.FindElement(
            By.CssSelector(".fullname"));

        public string Name {
            get
            {
                return NameInputElement.GetAttribute("value");
            }
            set
            {
                NameInputElement.Clear();
                NameInputElement.SendKeys(value);
            }
        }

        public string Email {
            get
            {
                return EmailInputElement.GetAttribute("value");
            }
            set
            {
                EmailInputElement.Clear();
                EmailInputElement.SendKeys(value);
            }
        }

        public string Enquiry {
            get
            {
                return EnquiryInputElement.GetAttribute("value");
            }
            set
            {
                EnquiryInputElement.Clear();
                EnquiryInputElement.SendKeys(value);
            }
        }

        public ContactUsModel ViewModel
        {
            get
            {
                var model = new ContactUsModel
                {

                    // TODO: Get selector for when this is visible.
                    DisplayCaptcha = false,

                    Email = Email,
                    Enquiry = Enquiry,
                    FullName = Name
                };

                return model;
            }
        }

        #endregion

        #region Methods

        public ContactUsSubmittedPage Submit()
        {
            SubmitButton.Click();
            return new ContactUsSubmittedPage(WrappedDriver);
        }

        #endregion
    }
}