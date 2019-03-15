using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApertureLabs.Selenium.Components.JQuery.TagEditor
{
    /// <summary>
    /// Represents the jQuery tagEditor widget.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// See https://goodies.pixabay.com/jquery/tag-editor/demo.html for more
    /// information.
    /// </remarks>
    /// <seealso cref="ApertureLabs.Selenium.Components.JQuery.JQueryWidgetBase{T}" />
    public class TagEditorComponent<T> : JQueryWidgetBase<T>
    {
        #region Fields

        #region Selectors

        private readonly By selectedTagsSelector = By.CssSelector(".tag-editor-tag");

        #endregion

        private readonly TagEditorConfiguration tagEditorConfiguration;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TagEditorComponent{T}"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="driver">The driver.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="tagEditorConfiguration">The tag editor configuration.</param>
        /// <exception cref="ArgumentNullException">tagEditorConfiguration</exception>
        public TagEditorComponent(By selector,
            IWebDriver driver,
            T parent,
            TagEditorConfiguration tagEditorConfiguration)
            : base(selector,
                  driver,
                  parent)
        {
            this.tagEditorConfiguration = tagEditorConfiguration
                ?? throw new ArgumentNullException(nameof(tagEditorConfiguration));
        }

        #endregion

        #region Properties

        #region Elements

        private IWebElement TagEditorContainerElement { get; set; }

        private IReadOnlyCollection<IWebElement> SelectedTagElements =>
            TagEditorContainerElement.FindElements(selectedTagsSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// If overriding don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidElementStateException">
        /// Element is missing the id attribute.
        /// </exception>
        public override ILoadableComponent Load()
        {
            base.Load();

            var id = WrappedElement.GetAttribute("id");

            if (String.IsNullOrEmpty(id))
            {
                throw new InvalidElementStateException("Element is missing " +
                    "the id attribute.");
            }

            TagEditorContainerElement = WrappedElement.FindElement(
                By.CssSelector($"*[data-valmsg-for='{id}']"));

            return this;
        }

        /// <summary>
        /// Gets all availble tags that can be created via autocomplete.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllTags()
        {
            var js = new JavaScript
            {
                Script =
                    "var $el = $(arguments[0]);" +
                    "return $el.data().options.autocomplete.source;",
                Arguments = new[] { new JavaScriptValue(WrappedElement) }
            };

            var result = js.Execute(WrappedDriver.JavaScriptExecutor());

            return result.ToStringArray();
        }

        /// <summary>
        /// Gets the selected tags.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetSelectedTags()
        {
            foreach (var tagEl in SelectedTagElements)
            {
                var text = tagEl.TextHelper().InnerText;

                yield return text;
            }
        }

        /// <summary>
        /// Determines whether any selected tags matches the specified tag text.
        /// </summary>
        /// <param name="tagText">The tag text.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if any selected tags matches the specified tag text;
        ///   otherwise, <c>false</c>.
        /// </returns>
        public bool IsTagSelected(string tagText,
                    StringComparison stringComparison = StringComparison.Ordinal)
        {
            return GetSelectedTags()
                .Any(tag => String.Equals(tag, tagText, stringComparison));
        }

        /// <summary>
        /// Selects the tag.
        /// </summary>
        /// <param name="tagText">The tag text.</param>
        /// <returns></returns>
        public TagEditorComponent<T> SelectTag(string tagText)
        {
            var selectedTags = SelectedTagElements;

            if (selectedTags.Any())
            {
                var lastActiveEl = selectedTags.Last();
                var width = lastActiveEl.Size.Width;

                WrappedDriver.CreateActions()
                    .MoveToElement(lastActiveEl)
                    .MoveByOffset(width, 0)
                    .Click()
                    .SendKeys(tagText + Keys.Enter)
                    .Perform();
            }
            else
            {
                WrappedDriver.CreateActions()
                    .MoveToElement(TagEditorContainerElement)
                    .Click()
                    .SendKeys(tagText + Keys.Enter)
                    .Perform();
            }

            return this;
        }

        /// <summary>
        /// Deselects the tag.
        /// </summary>
        /// <param name="tagText">The tag text.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns></returns>
        public TagEditorComponent<T> DeselectTag(string tagText,
                    StringComparison stringComparison = StringComparison.Ordinal)
        {
            var tagEl = SelectedTagElements.FirstOrDefault(
                e => String.Equals(
                    e.TextHelper().InnerText,
                    tagText,
                    stringComparison));

            if (tagEl == null)
                throw new NoSuchElementException();

            if (tagEditorConfiguration.UseKeyboardInsteadOfMouseWhenInteracting)
            {
                WrappedDriver.CreateActions()
                    .MoveToElement(tagEl)
                    .Click()
                    .SendKeys(Keys.LeftControl + "a")
                    .SendKeys(Keys.Delete)
                    .SendKeys(Keys.Enter)
                    .SendKeys(Keys.Escape)
                    .Perform();
            }
            else
            {
                tagEl
                    .GetParentElement()
                    .FindElement(By.CssSelector(".tag-editor-delete"))
                    .Click();
            }

            WrappedDriver
                .Wait(TimeSpan.FromSeconds(1))
                .Until(d => tagEl.IsStale());

            return this;
        }

        #endregion
    }
}
