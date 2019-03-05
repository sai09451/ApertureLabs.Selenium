using System;
using OpenQA.Selenium;

namespace ApertureLabs.Selenium.Components.JQuery.Accordian
{
    /// <summary>
    /// Options for configuring the AccordianComponent and how Selenium
    /// interacts with it.
    /// </summary>
    public class AccordionComponentOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccordionComponentOptions"/> class.
        /// </summary>
        public AccordionComponentOptions()
        {
            AccordianHeaderClass = By.CssSelector(".ui-corner-top");
            AccordianHeaderCollapsedClass = By.CssSelector(".ui-corner-all");
            AccordianContentClass = By.CssSelector(".ui-corner-bottom");
            AnimationDuration = TimeSpan.FromSeconds(2);
            Collaspable = false;
            Event = "click";
        }

        /// <summary>
        /// Gets or sets the accordian header class.
        /// </summary>
        /// <value>
        /// The accordian header class.
        /// </value>
        public By AccordianHeaderClass { get; set; }

        /// <summary>
        /// Gets or sets the accordian header collapsed class.
        /// </summary>
        /// <value>
        /// The accordian header collapsed class.
        /// </value>
        public By AccordianHeaderCollapsedClass { get; set; }

        /// <summary>
        /// Gets or sets the accordian content class.
        /// </summary>
        /// <value>
        /// The accordian content class.
        /// </value>
        public By AccordianContentClass { get; set; }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        /// <value>
        /// The duration of the animation.
        /// </value>
        public TimeSpan AnimationDuration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="AccordionComponentOptions"/> can have all panels
        /// collapsed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if collaspable; otherwise, <c>false</c>.
        /// </value>
        public bool Collaspable { get; set; }

        /// <summary>
        /// Gets or sets the event that triggers the panel. Default is 'click'.
        /// </summary>
        /// <value>
        /// The event.
        /// </value>
        public string Event { get; set; }
    }
}
