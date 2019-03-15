using System;
using System.Collections.Generic;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.Boostrap.DropDown
{
    /// <summary>
    /// The bootstrap dropdown component.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class DropDownComponent : PageComponent
    {
        #region Fields

        #region Selectors

        private readonly By allItemsSelector = By.CssSelector(".dropdown-item");
        private readonly By enabledItemsSelector = By.CssSelector(".dropdown-item:not(.disabled)");
        private readonly By activeItemSelector = By.CssSelector(".dropdown-item.active");
        private readonly By toggleDropDownSelector = By.CssSelector(".dropdown-toggle");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownComponent"/> class.
        /// </summary>
        /// <param name="selector">
        /// The selector. Should point to an element that has the btn-group
        /// class applied to it.
        /// </param>
        /// <param name="driver">The driver.</param>
        public DropDownComponent(By selector, IWebDriver driver)
            : base(selector, driver)
        { }

        #endregion

        #region Properties

        #region Elements

        private IWebElement ToggleDropDownElement => WrappedDriver
            .FindElement(toggleDropDownSelector);

        private IReadOnlyCollection<IWebElement> EnabledItemElements => WrappedDriver
            .FindElements(enabledItemsSelector);

        private IReadOnlyCollection<IWebElement> AllItems => WrappedDriver
            .FindElements(allItemsSelector);

        private IWebElement ActiveItemElement => WrappedDriver
            .FindElement(activeItemSelector);

        #endregion

        private bool IsSplitButton { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// If overriding don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidElementStateException">Expected the class " +
        ///                     "'btn-group' to be present on the WrappedElement.</exception>
        public override ILoadableComponent Load()
        {
            base.Load();

            // Verify this is a button group.
            if (!WrappedElement.Is(By.CssSelector(".btn-group")))
            {
                throw new InvalidElementStateException("Expected the class " +
                    "'btn-group' to be present on the WrappedElement.");
            }

            // Check if split or single.
            IsSplitButton = WrappedElement
                .FindElements(By.CssSelector(".dropdown-toggle-split"))
                .Any();

            return this;
        }

        /// <summary>
        /// Expands the dropdown if not already expanded.
        /// </summary>
        /// <returns></returns>
        public virtual DropDownComponent Expand()
        {
            if (!IsExpanded())
                ToggleDropDownElement.Click();

            return this;
        }

        /// <summary>
        /// Collapses the dropdown if not already collapsed.
        /// </summary>
        /// <returns></returns>
        public virtual DropDownComponent Collapse()
        {
            if (IsExpanded())
                ToggleDropDownElement.Click();

            return this;
        }

        /// <summary>
        /// Gets trimmed inner text of the main button.
        /// </summary>
        /// <returns></returns>
        public virtual string GetName()
        {
            return WrappedElement
                .Children()
                .First()
                .TextHelper()
                .InnerText;
        }

        /// <summary>
        /// Returns the enabled drop down elements (doesn't include dividers).
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetEnabledItems()
        {
            return EnabledItemElements;
        }

        /// <summary>
        /// Returns all item elements (doesn't include dividers).
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyCollection<IWebElement> GetAllItems()
        {
            return AllItems;
        }

        private bool IsExpanded()
        {
            return WrappedElement.Is(By.CssSelector(".open"));
        }

        #endregion
    }
}
