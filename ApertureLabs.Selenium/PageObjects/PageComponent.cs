using System;
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
        /// Initializes a new instance of the <see cref="PageComponent"/> class.
        /// </summary>
        /// <param name="by">The by.</param>
        /// <param name="driver">The driver.</param>
        /// <exception cref="ArgumentNullException">
        /// by
        /// or
        /// driver
        /// </exception>
        public PageComponent(By by, IWebDriver driver)
        {
            if (by == null)
                throw new ArgumentNullException(nameof(by));
            else if (driver == null)
                throw new ArgumentNullException(nameof(driver));

            this.By = by;
            this.WrappedDriver = driver;
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
