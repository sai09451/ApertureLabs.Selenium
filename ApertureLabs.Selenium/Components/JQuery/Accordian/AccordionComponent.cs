using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.JQuery.Accordian
{
    /// <summary>
    /// AccordionComponent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="JQueryWidgetBase{T}" />
    public class AccordionComponent<T> : JQueryWidgetBase<T>
    {
        #region Fields

        private readonly AccordionComponentOptions accordianComponentOptions;
        private readonly IPageObjectFactory pageObjectFactory;

        #region Selectors

        private readonly By panelsSelector = By.CssSelector(".ui-accordion-header");
        private readonly By activePanelSelector = By.CssSelector(".ui-accordion-header-active");
        private readonly By activeContentSelector = By.CssSelector(".ui-accordion-content-active");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionComponent{T}"/> class.
        /// </summary>
        /// <param name="accordianComponentOptions">The options.</param>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="parent">The parent.</param>
        public AccordionComponent(
            AccordionComponentOptions accordianComponentOptions,
            By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver,
            T parent)
            : base(selector, driver, parent)
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

        private IWebElement ActivePanelElement => WrappedElement.FindElements(activePanelSelector).FirstOrDefault();
        private IWebElement ActiveContentElement => WrappedElement.FindElements(activeContentSelector).FirstOrDefault();
        private IReadOnlyCollection<IWebElement> PanelElements => WrappedElement.FindElements(panelsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// If overloaded don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        public override ILoadableComponent Load()
        {
            base.Load();

            WaitForInitialization("uiAccordion", TimeSpan.FromSeconds(2));

            return this;
        }

        /// <summary>
        /// Gets all of the panel names.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<string> GetPanelNames()
        {
            return PanelElements
                .Select(e => e.TextHelper().InnerText)
                .ToList();
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
            var isOpen = String.Equals(
                GetActivePanelName(),
                panelName,
                stringComparison);

            if (!isOpen)
            {
                var matchingPanelElement = PanelElements.FirstOrDefault(
                    e => String.Equals(
                        e.TextHelper().InnerText,
                        panelName,
                        stringComparison));

                if (matchingPanelElement == null)
                    throw new NoSuchElementException();

                var waiter = WrappedElement.GetEventWaiter("accordionactivate");

                switch (accordianComponentOptions.Event)
                {
                    case "click":
                        matchingPanelElement.Click();
                        break;
                    case "mouseover":
                        WrappedDriver.CreateActions()
                            .MoveToElement(matchingPanelElement)
                            .Perform();
                        break;
                    default:
                        throw new NotImplementedException(accordianComponentOptions.Event);
                }

                waiter.Wait(accordianComponentOptions.AnimationDuration);
            }

            return ActiveContentElement;
        }

        /// <summary>
        /// Closes the active panel.
        /// </summary>
        public void ClosePanel()
        {
            if (!accordianComponentOptions.Collaspable)
            {
                throw new Exception("The " +
                    "AccordianComponentOptions.Collaspable was set to false.");
            }

            if (HasOpenPanel())
            {
                var waiter = WrappedElement.GetEventWaiter("accordionactivate");

                switch (accordianComponentOptions.Event)
                    {
                        case "click":
                            ActivePanelElement.Click();
                            break;
                        case "mouseover":
                            WrappedDriver.CreateActions()
                                .MoveToElement(ActivePanelElement)
                                .Perform();
                            break;
                        default:
                            throw new NotImplementedException(accordianComponentOptions.Event);
                    }

                waiter.Wait(accordianComponentOptions.AnimationDuration);
            }
        }

        /// <summary>
        /// Determines whether their is an open panel.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [has open panel]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOpenPanel()
        {
            return ActivePanelElement != null;
        }

        /// <summary>
        /// Gets the name of the active panel.
        /// </summary>
        /// <returns></returns>
        public string GetActivePanelName()
        {
            return ActivePanelElement.TextHelper().InnerText;
        }

        /// <summary>
        /// Gets the active panels content element.
        /// </summary>
        /// <returns></returns>
        public IWebElement GetActivePanelContentElement()
        {
            if (!HasOpenPanel())
                return null;

            return ActiveContentElement;
        }

        #endregion
    }
}
