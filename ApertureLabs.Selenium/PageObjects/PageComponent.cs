using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Default implementation of IPageComponent.
    /// </summary>
    public abstract class PageComponent : IPageComponent
    {
        #region Constructor

        public PageComponent(IWebDriver driver, By by)
        {
            By = by;
            WrappedDriver = driver;
        }

        #endregion

        #region Properties

        public IWebDriver WrappedDriver { get; private set; }

        public IWebElement WrappedElement { get; private set; }

        public By By { get; private set; }

        #endregion

        #region Methods

        public abstract bool IsStale();

        /// <summary>
        /// If overloaded don't forget to call base.Load().
        /// </summary>
        /// <returns></returns>
        public virtual ILoadableComponent Load()
        {
            WrappedElement = WrappedDriver.FindElement(By);
            return this;
        }

        #endregion
    }
}
