using Aperture.NopPageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using System;

namespace Aperture.Nop400.PageObjects.Public.Common
{
    public class ContactUsPage : BasePage<string>
    {
        #region Fields

        #endregion

        #region Constructor

        public ContactUsPage(IWebDriver driver) : base(driver)
        { }

        #endregion

        #region Properties

        private IWebElement emailInputElement => Driver.FindElement(By.CssSelector(".email"));

        private IWebElement enquiryInputElement => Driver.FindElement(By.CssSelector(".enquiry"));

        private IWebElement submitButton => Driver.FindElement(By.CssSelector("[name='send-email']"));

        private IWebElement nameInputElement => Driver.FindElement(By.CssSelector(".fullname"));

        public string Name {
            get
            {
                return nameInputElement.GetAttribute("value");
            }
            set
            {
                nameInputElement.Clear();
                nameInputElement.SendKeys(value);
            }
        }

        public string Email {
            get
            {
                return emailInputElement.GetAttribute("value");
            }
            set
            {
                emailInputElement.Clear();
                emailInputElement.SendKeys(value);
            }
        }

        public string Enquiry {
            get
            {
                return enquiryInputElement.GetAttribute("value");
            }
            set
            {
                enquiryInputElement.Clear();
                enquiryInputElement.SendKeys(value);
            }
        }

        #endregion

        #region Methods

        public ContactUsPage Submit()
        {
            submitButton.Click();
            return new ContactUsPageSubmitted(Driver);
        }

        #endregion
    }
}