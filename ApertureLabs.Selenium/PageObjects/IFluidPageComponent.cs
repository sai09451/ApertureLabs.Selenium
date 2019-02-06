using System;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Similar to IPageComponent but adds a method for retrieving the parent
    /// object. This allows for chaining back to the parent object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IPageComponent" />
    public interface IFluidPageComponent<T> : IPageComponent
    {
        /// <summary>
        /// Retrieves the parent/container of this component.
        /// </summary>
        /// <returns></returns>
        T Parent();

        /// <summary>
        /// Used for performing actions on the component that may not be
        /// chainable then returns the component.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        IFluidPageComponent<T> Perform(Action<IFluidPageComponent<T>> action);
    }
}
