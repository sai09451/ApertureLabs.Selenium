using ApertureLabs.Selenium.Extensions;
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
        public IWebDriver WrappedDriver { get; protected set; }

        /// <summary>
        /// The element representing the component.
        /// </summary>
        public IWebElement WrappedElement { get; protected set; }

        /// <inheritdoc/>
        public By By { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Used to check if the component is still valid for use.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsStale()
        {
            return WrappedElement?.IsStale() ?? true;
        }

        /// <summary>
        /// If overloaded don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
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
