﻿namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Used for <see cref="IPageObject"/> or <see cref="IPageComponent"/>
    /// which use a model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IViewModel<T>
    {
        /// <summary>
        /// Retrieves the viewmodel.
        /// </summary>
        T ViewModel();
    }
}
