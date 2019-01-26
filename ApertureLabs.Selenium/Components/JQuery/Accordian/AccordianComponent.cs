using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.JQuery.Accordian
{
    /// <summary>
    /// AccordianComponent.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class AccordianComponent : PageComponent
    {
        #region Fields

        private readonly AccordianComponentOptions accordianComponentOptions;
        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordianComponent"/> class.
        /// </summary>
        /// <param name="accordianComponentOptions">The options.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        public AccordianComponent(
            AccordianComponentOptions accordianComponentOptions,
            By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(driver, selector)
        {
            this.accordianComponentOptions = accordianComponentOptions
                ?? throw new ArgumentNullException(nameof(accordianComponentOptions));
            this.pageObjectFactory = pageObjectFactory
                ?? throw new ArgumentNullException(nameof(pageObjectFactory));

            // Validate the 'Event' property on the accordianComponentOptions.
            var supportedEvents = new[]
            {
                "mouseover",
                "click"
            };

            if (!supportedEvents.Contains(accordianComponentOptions.Event))
                throw new NotImplementedException(nameof(accordianComponentOptions.Event));
        }

        #endregion

        #region Properties

        #region Elements

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets all of the panel names.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetPanelNames()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the panel to open and returns the panel content element.
        /// </summary>
        /// <param name="panelName">Name of the panel.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public IWebElement SelectPanel(string panelName,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the active panel.
        /// </summary>
        public void ClosePanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether their is an open panel.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has open panel]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOpenPanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the name of the active panel.
        /// </summary>
        /// <returns></returns>
        public string GetActivePanelName()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the active panels content element.
        /// </summary>
        /// <returns></returns>
        public IWebElement GetActivePanelContentElement()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
