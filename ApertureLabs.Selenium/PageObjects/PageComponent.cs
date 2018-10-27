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

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        public PageComponent(IWebDriver driver, By by)
        {
            By = by;
            WrappedDriver = driver;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The IWebDriver of a component.
        /// </summary>
        public IWebDriver WrappedDriver { get; private set; }

        /// <summary>
        /// The element representing the component.
        /// </summary>
        public IWebElement WrappedElement { get; private set; }

        /// <inheritdoc/>
        public By By { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Used to check if the component is still valid for use.
        /// </summary>
        /// <returns></returns>
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
