using System;
using ApertureLabs.Selenium.Attributes;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Default implementation of <see cref="IFluidPageComponent{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IFluidPageComponent{T}" />
    public class FluidPageComponent<T> : PageComponent, IFluidPageComponent<T>
    {
        #region Fields

        private readonly T parent;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FluidPageComponent{T}"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="parent">The parent. Can be null.</param>
        public FluidPageComponent(
            By selector,
            IWebDriver driver,
            T parent)
            : base(selector, driver)
        {
            this.parent = parent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the parent/container of this component.
        /// </summary>
        /// <returns></returns>
        public virtual T Parent()
        {
            return parent;
        }

        /// <summary>
        /// Used for performing actions on the component that may not be
        /// chainable then returns the component.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public virtual IFluidPageComponent<T> Perform(Action<IFluidPageComponent<T>> action)
        {
            action(this);

            return this;
        }

        #endregion
    }
}
