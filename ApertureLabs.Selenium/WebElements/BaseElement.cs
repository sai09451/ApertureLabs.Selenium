using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;

namespace ApertureLabs.Selenium.WebElements
{
    /// <summary>
    /// Base class for element wrappers.
    /// </summary>
    /// <seealso cref="OpenQA.Selenium.Internal.IWrapsElement" />
    /// <seealso cref="OpenQA.Selenium.IWebElement" />
    public abstract class BaseElement : IWrapsElement, IWebElement
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseElement"/> class.
        /// </summary>
        /// <param name="element">The element.</param>
        public BaseElement(IWebElement element)
        {
            WrappedElement = element;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="T:OpenQA.Selenium.IWebElement" /> wrapped by this object.
        /// </summary>
        public IWebElement WrappedElement { get; }

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        /// <remarks>
        /// The <see cref="P:OpenQA.Selenium.IWebElement.TagName" /> property returns the tag name of the
        /// element, not the value of the name attribute. For example, it will return
        /// "input" for an element specified by the HTML markup &lt;input name="foo" /&gt;.
        /// </remarks>
        public string TagName => WrappedElement.TagName;

        /// <summary>
        /// Gets the innerText of this element, without any leading or trailing whitespace,
        /// and with other whitespace collapsed.
        /// </summary>
        public string Text => WrappedElement.Text;

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        /// <remarks>
        /// The <see cref="P:OpenQA.Selenium.IWebElement.Enabled" /> property will generally
        /// return <see langword="true" /> for everything except explicitly disabled input elements.
        /// </remarks>
        public bool Enabled => WrappedElement.Enabled;

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        /// <remarks>
        /// This operation only applies to input elements such as checkboxes,
        /// options in a select element and radio buttons.
        /// </remarks>
        public bool Selected => WrappedElement.Selected;

        /// <summary>
        /// Gets a <see cref="T:System.Drawing.Point" /> object containing the coordinates of the upper-left corner
        /// of this element relative to the upper-left corner of the page.
        /// </summary>
        public Point Location => WrappedElement.Location;

        /// <summary>
        /// Gets a <see cref="P:OpenQA.Selenium.IWebElement.Size" /> object containing the height and width of this element.
        /// </summary>
        public Size Size => WrappedElement.Size;

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        /// <remarks>
        /// The <see cref="P:OpenQA.Selenium.IWebElement.Displayed" /> property avoids the problem
        /// of having to parse an element's "style" attribute to determine
        /// visibility of an element.
        /// </remarks>
        public bool Displayed => WrappedElement.Displayed;

        #endregion

        #region Methods

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        /// <remarks>
        /// If this element is a text entry element, the <see cref="M:OpenQA.Selenium.IWebElement.Clear" />
        /// method will clear the value. It has no effect on other elements. Text entry elements
        /// are defined as elements with INPUT or TEXTAREA tags.
        /// </remarks>
        public void Clear()
        {
            WrappedElement.Clear();
        }

        /// <summary>
        /// Clicks this element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Click this element. If the click causes a new page to load, the <see cref="M:OpenQA.Selenium.IWebElement.Click" />
        /// method will attempt to block until the page has loaded. After calling the
        /// <see cref="M:OpenQA.Selenium.IWebElement.Click" /> method, you should discard all references to this
        /// element unless you know that the element and the page will still be present.
        /// Otherwise, any further operations performed on this element will have an undefined.
        /// behavior.
        /// </para>
        /// <para>
        /// If this element is not clickable, then this operation is ignored. This allows you to
        /// simulate a users to accidentally missing the target when clicking.
        /// </para>
        /// </remarks>
        public void Click()
        {
            WrappedElement.Click();
        }

        /// <summary>
        /// Finds the first <see cref="T:OpenQA.Selenium.IWebElement" /> using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>
        /// The first matching <see cref="T:OpenQA.Selenium.IWebElement" /> on the current context.
        /// </returns>
        public IWebElement FindElement(By by)
        {
            return WrappedElement.FindElement(by);
        }

        /// <summary>
        /// Finds all <see cref="T:OpenQA.Selenium.IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>
        /// A <see cref="T:System.Collections.ObjectModel.ReadOnlyCollection`1" /> of all <see cref="T:OpenQA.Selenium.IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.
        /// </returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return WrappedElement.FindElements(by);
        }

        /// <summary>
        /// Gets the value of the specified attribute for this element.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>
        /// The attribute's current value. Returns a <see langword="null" /> if the
        /// value is not set.
        /// </returns>
        /// <remarks>
        /// The <see cref="M:OpenQA.Selenium.IWebElement.GetAttribute(System.String)" /> method will return the current value
        /// of the attribute, even if the value has been modified after the page has been
        /// loaded. Note that the value of the following attributes will be returned even if
        /// there is no explicit attribute on the element:
        /// <list type="table"><listheader><term>Attribute name</term><term>Value returned if not explicitly specified</term><term>Valid element types</term></listheader><item><description>checked</description><description>checked</description><description>Check Box</description></item><item><description>selected</description><description>selected</description><description>Options in Select elements</description></item><item><description>disabled</description><description>disabled</description><description>Input and other UI elements</description></item></list>
        /// </remarks>
        public string GetAttribute(string attributeName)
        {
            return WrappedElement.GetAttribute(attributeName);
        }

        /// <summary>
        /// Gets the value of a CSS property of this element.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property to get the value of.</param>
        /// <returns>
        /// The value of the specified CSS property.
        /// </returns>
        /// <remarks>
        /// The value returned by the <see cref="M:OpenQA.Selenium.IWebElement.GetCssValue(System.String)" />
        /// method is likely to be unpredictable in a cross-browser environment.
        /// Color values should be returned as hex strings. For example, a
        /// "background-color" property set as "green" in the HTML source, will
        /// return "#008000" for its value.
        /// </remarks>
        public string GetCssValue(string propertyName)
        {
            return WrappedElement.GetCssValue(propertyName);
        }

        /// <summary>
        /// Gets the value of a JavaScript property of this element.
        /// </summary>
        /// <param name="propertyName">The name JavaScript the JavaScript property to get the value of.</param>
        /// <returns>
        /// The JavaScript property's current value. Returns a <see langword="null" /> if the
        /// value is not set or the property does not exist.
        /// </returns>
        public string GetProperty(string propertyName)
        {
            return WrappedElement.GetProperty(propertyName);
        }

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        /// <param name="text">The text to type into the element.</param>
        /// <remarks>
        /// The text to be typed may include special characters like arrow keys,
        /// backspaces, function keys, and so on. Valid special keys are defined in
        /// <see cref="T:OpenQA.Selenium.Keys" />.
        /// </remarks>
        /// <seealso cref="T:OpenQA.Selenium.Keys" />
        public void SendKeys(string text)
        {
            WrappedElement.SendKeys(text);
        }

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        /// <remarks>
        /// If this current element is a form, or an element within a form,
        /// then this will be submitted to the web server. If this causes the current
        /// page to change, then this method will block until the new page is loaded.
        /// </remarks>
        public void Submit()
        {
            WrappedElement.Submit();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return WrappedElement.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return WrappedElement.GetHashCode();
        }

        #endregion
    }
}
