using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace ApertureLabs.Selenium.PageObjects
{
    /// <summary>
    /// Used for creating 'parent' IPageObjects used for
    /// inheritance/composition. The only noticable changes are that the
    /// <see cref="BaseUri"/> and <see cref="Route"/> will always be null and
    /// calling <see cref="Load"/> won't do any validation.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageObject" />
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.IPageObject" />
    public abstract class BasePageObject : PageObject, IPageObject
    {
        #region Constructor

        public BasePageObject(IWebDriver driver)
            : base(driver,
                  new Uri(".", UriKind.Relative),
                  new UriTemplate(""))
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Overridden so it will always be null.
        /// </value>
        public override Uri BaseUri
        {
            get => null;
            protected set => base.BaseUri = value;
        }

        /// <summary>
        /// Overridden so it will always be null.
        /// </summary>
        public override UriTemplate Route
        {
            get => base.Route;
            protected set => base.Route = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the component. Overridden so it doesn't validate the
        /// <see cref="BasePageObject.Route"/>. Calling this will set the
        /// <see cref="BasePageObject.Uri"/> and
        /// <see cref="BasePageObject.WindowHandle"/> properties.
        /// </summary>
        /// <returns>
        /// A reference to this
        /// <see cref="T:OpenQA.Selenium.Support.UI.ILoadableComponent" />.
        /// </returns>
        public override ILoadableComponent Load()
        {
            Uri = new Uri(WrappedDriver.Url);
            WindowHandle = WrappedDriver.CurrentWindowHandle;

            return this;
        }

        #endregion
    }
}
