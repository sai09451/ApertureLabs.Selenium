using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.Properties;
using ApertureLabs.Selenium.WebElements.IFrame;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;
using System.Linq;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Used for interacting with TinyMCE editors.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class TinyMCEComponent : PageComponent
    {
        #region Fields

        private readonly TinyMCEOptions tinyMCEOptions;
        private readonly IPageObjectFactory pageObjectFactory;
        private IFrameElement iframeElement;

        #region Selectors

        private readonly By menuComponentSelector = By.CssSelector("*[role='menubar']");
        private readonly By toolbarComponentSelector = By.CssSelector("*[role='toolbar']");
        private readonly By statusbarComponentSelector = By.CssSelector(".mce-statusbar");
        private readonly By editableBodySelector = By.CssSelector(".mce-edit-area");
        private readonly By resizeHandleSelector = By.CssSelector(".mce-i-resize");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyMCEComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        /// <param name="tinyMCEOptions">The options.</param>
        public TinyMCEComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver,
            TinyMCEOptions tinyMCEOptions)
            : base(selector, driver)
        {
            this.pageObjectFactory = pageObjectFactory;
            this.tinyMCEOptions = tinyMCEOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the integration mode.
        /// </summary>
        public IntegrationMode IntegrationMode { get; private set; }

        /// <summary>
        /// Used to manipulate the menu area. Null if no menu bar.
        /// </summary>
        public MenuComponent Menu { get; protected set; }

        /// <summary>
        /// Used to manipulate the toolbar. Null if no toolbar.
        /// </summary>
        public ToolbarComponent Toolbar { get; protected set; }

        /// <summary>
        /// Used to manipulate the statusbar. Null if no status bar.
        /// </summary>
        public StatusbarComponent Statusbar { get; protected set; }

        #region Elements

        /// <summary>
        /// Gets or sets the element created by tinyMCE.
        /// </summary>
        private IWebElement TinyMCEContainerElement { get; set; }

        private IWebElement EditableBodyElement => IntegrationMode == IntegrationMode.Classic
            ? WrappedDriver.FindElement(By.TagName("body"))
            : TinyMCEContainerElement.FindElement(editableBodySelector);

        private IWebElement ResizeHandleElement => WrappedDriver.FindElement(resizeHandleSelector);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// If overriding don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NoSuchElementException"></exception>
        public override ILoadableComponent Load()
        {
            base.Load();

            WaitForInitalization();

            var script = new JavaScript
            {
                Arguments = new[] { new JavaScriptValue(WrappedElement) },
                IsAsync = false,
                Script = JavaScript.Clean(
                    JavaScript.RemoveComments(
                        AddTinyMCEUtilities() +
                        "var el = arguments[0];" +
                        "var editor = tinyMCEUtilities.getEditor(el);" +
                        "return editor == null ? null : editor.getContainer();"))
            };

            TinyMCEContainerElement = script.Execute(
                    WrappedDriver.JavaScriptExecutor())
                .ToWebElement();

            if (TinyMCEContainerElement == null)
                throw new NoSuchElementException();

            UpdateIntegrationMode();

            if (IntegrationMode == IntegrationMode.Classic)
            {
                iframeElement = new IFrameElement(
                    TinyMCEContainerElement
                        .FindElement(By.TagName("iframe"))
                        .UnWrapEventFiringWebElement(),
                    WrappedDriver);
            }

            // Menu.
            if (TinyMCEContainerElement.FindElements(menuComponentSelector).Any())
            {
                Menu = pageObjectFactory.PrepareComponent(
                    new MenuComponent(menuComponentSelector,
                        pageObjectFactory,
                        WrappedDriver));
            }
            else
            {
                Menu = null;
            }

            // Toolbar.
            if (TinyMCEContainerElement.FindElements(toolbarComponentSelector).Any())
            {
                Toolbar = pageObjectFactory.PrepareComponent(
                    new ToolbarComponent(
                        toolbarComponentSelector,
                        pageObjectFactory,
                        WrappedDriver));
            }
            else
            {
                Toolbar = null;
            }

            // Status bar.
            if (TinyMCEContainerElement.FindElements(statusbarComponentSelector).Any())
            {
                Statusbar = pageObjectFactory.PrepareComponent(
                    new StatusbarComponent(statusbarComponentSelector,
                        pageObjectFactory,
                        WrappedDriver));
            }
            else
            {
                Statusbar = null;
            }

            return this;
        }

        /// <summary>
        /// Sets the size of the editor.
        /// </summary>
        /// <param name="desiredSize">Size of the desired.</param>
        public virtual void SetEditorSize(Size desiredSize)
        {
            var currentDimensions = TinyMCEContainerElement.Size;
            var diff = Size.Subtract(desiredSize, currentDimensions);

            WrappedDriver.CreateActions()
                .MoveToElement(ResizeHandleElement)
                .ClickAndHold()
                .MoveByOffset(diff.Width, diff.Height)
                .Release()
                .Perform();
        }

        /// <summary>
        /// Gets the size of the editor.
        /// </summary>
        /// <returns></returns>
        public virtual Size GetEditorSize()
        {
            return TinyMCEContainerElement.Size;
        }

        /// <summary>
        /// Gets the cursor position.
        /// </summary>
        /// <returns></returns>
        public virtual Point GetCursorPosition()
        {
            var script =
                AddTinyMCEUtilities() +
                "var el = arguments[0];" +
                "var ed = tinyMCEUtilities.getEditor(el);" +
                "console.log(ed);" +
                "return JSON.stringify(tinyMCEUtilities.getCaretPosition(ed))";

            script = JavaScript.RemoveComments(script);
            script = JavaScript.Clean(script);

            var result = (string)WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);

            var jsObj = JObject.Parse(result);

            var point = new Point(
                jsObj["x"].ToObject<int>(),
                jsObj["y"].ToObject<int>());

            return point;
        }

        /// <summary>
        /// Sets the cursor position (the point {0,0} will be the top left
        /// corner).
        /// </summary>
        /// <param name="point">The point.</param>
        public virtual void SetCursorPosition(Point point)
        {
            var script =
                AddTinyMCEUtilities() +
                $"var el = arguments[0];" +
                $"var editor = tinyMCEUtilities.getEditor(el);" +
                $"tinyMCEUtilities.setCaretPosition(editor, {point.X}, {point.Y});" +
                // This is to scroll the line into view.
                $"editor.execCommand('mceInsertContent', false, '');";

            WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);
        }

        /// <summary>
        /// Highlights all text.
        /// </summary>
        public virtual void HighlightAllText()
        {
            if (IntegrationMode == IntegrationMode.Classic)
                iframeElement.InFrameAction(_HighlightAllText);
            else
                _HighlightAllText();
        }

        /// <summary>
        /// Hightlights the range.
        /// </summary>
        /// <param name="startPoint">The start.</param>
        /// <param name="endPoint">The end.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if the start point is before the end point.
        /// </exception>
        public virtual void HightlightRange(Point startPoint, Point endPoint)
        {
            // Validate arguments.
            if (startPoint == null)
                throw new ArgumentNullException(nameof(startPoint));
            else if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));

            if (startPoint.Y > endPoint.Y)
                throw new ArgumentOutOfRangeException(nameof(startPoint));
            else if (startPoint.Y == endPoint.Y && startPoint.X > endPoint.X)
                throw new ArgumentOutOfRangeException(nameof(endPoint));

            // This is to set the start point & end point into view. Both
            // points are used to (if possible) get both points into view.
            SetCursorPosition(startPoint);
            SetCursorPosition(endPoint);

            var script =
                AddTinyMCEUtilities() +
                $"var el = arguments[0];" +
                $"var editor = tinyMCEUtilities.getEditor(el);" +
                $"tinyMCEUtilities.highlight(editor," +
                    $"{startPoint.X}," +
                    $"{startPoint.Y}," +
                    $"{endPoint.X}," +
                    $"{endPoint.Y});";

            WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);
        }

        /// <summary>
        /// Gets the highlighted text.
        /// </summary>
        /// <returns></returns>
        public virtual string GetHighlightedText()
        {
            string text = IntegrationMode == IntegrationMode.Classic
                ? iframeElement.InFrameFunction(
                    () => WrappedDriver.GetHighlightedText())
                : WrappedDriver.GetHighlightedText();

            return text;
        }

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public virtual void Write(string content)
        {
            if (IntegrationMode == IntegrationMode.Classic)
                iframeElement.InFrameAction(() => _Write(content));
            else
                _Write(content);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="content">The content.</param>
        public virtual void WriteLine(string content)
        {
            Write(content + Keys.Enter);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public virtual void WriteLine()
        {
            Write(Keys.Enter);
        }

        /// <summary>
        /// Gets the content of the editor. This calls the
        /// tinyMCE.Editor.getContent().
        /// </summary>
        /// <returns></returns>
        public virtual string GetContent()
        {
            var script =
                AddTinyMCEUtilities() +
                "var el = arguments[0];" +
                "var editor = tinyMCEUtilities.getEditor(el);" +
                "return editor == null ? null : editor.getContent();";

            script = JavaScript.RemoveComments(script);
            script = JavaScript.Clean(script);

            return (string)WrappedDriver.JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);
        }

        /// <summary>
        /// Clears all content in the editable body area.
        /// </summary>
        public virtual void ClearAllContent()
        {
            if (IntegrationMode == IntegrationMode.Classic)
                iframeElement.InFrameAction(_ClearAllContent);
            else
                _ClearAllContent();
        }

        private void _HighlightAllText()
        {
            WrappedDriver.CreateActions()
                .MoveToElement(EditableBodyElement)
                .Click()
                .SendKeys(Keys.LeftControl + "a")
                .Perform();
        }

        private IWebElement GetRowElement(int row)
        {
            // Determine the row element.
            var rowEl = EditableBodyElement
                .Children()
                .ElementAt(row + 1);

            return rowEl;
        }

        private void _Write(string content)
        {
            WrappedDriver.CreateActions()
                .MoveToElement(EditableBodyElement)
                .Click()
                .SendKeys(content)
                .Perform();
        }

        private void _ClearAllContent()
        {
            WrappedDriver.CreateActions()
                .MoveToElement(EditableBodyElement)
                .Click()
                .SendKeys(Keys.LeftControl + 'a')
                .SendKeys(Keys.Delete)
                .Perform();
        }

        private void UpdateIntegrationMode()
        {
            if (TinyMCEContainerElement.FindElements(By.TagName("iframe")).Any())
            {
                // Only classic mode uses iframes.
                IntegrationMode = IntegrationMode.Classic;
            }
            else
            {
                // Could be either classic or distraction free.
                var script =
                    "var el = arguments[0];" +
                    "var editor = tinyMCEUtilities.getEditor(el);" +
                    "if (editor == null) {" +
                        "return false;" +
                    "} else {" +
                        "return editor.getParam('theme') === 'inlite';" +
                    "}";

                script = AddTinyMCEUtilities(script);

                var isDistractionFreeMode = (bool)WrappedDriver
                    .JavaScriptExecutor()
                    .ExecuteScript(script, WrappedElement);

                IntegrationMode = isDistractionFreeMode
                    ? IntegrationMode.DistractionFree
                    : IntegrationMode.Inline;
            }
        }

        private void WaitForInitalization()
        {
            var wait = WrappedDriver.Wait(TimeSpan.FromSeconds(10));
            var timeoutMS = wait.Timeout.TotalMilliseconds;
            var pollingMS = wait.PollingInterval.TotalMilliseconds;

            var script =
                AddTinyMCEUtilities(String.Empty) +
                "var el = {args}[0];" +
                "var editor = tinyMCEUtilities.getEditor(el);" +
                "tinyMCEUtilities.waitForInitialization(editor," +
                    $"{timeoutMS}," +
                    $"{pollingMS}," +
                    "{resolve}," +
                    "{reject});";

            script = JavaScript.RemoveComments(script);
            script = JavaScript.Clean(script);

            var waiter = new PromiseBody(WrappedDriver)
            {
                Arguments = new[] { new JavaScriptValue(WrappedElement) },
                Script = script
            };

            // Condense the script.
            waiter.Format();

            waiter.Execute(WrappedDriver.JavaScriptExecutor());
            wait.Until(d => waiter.Finished);
            waiter.Wait(TimeSpan.FromSeconds(10));
        }

        private static string AddTinyMCEUtilities(string script = null)
        {
            return Resources.tinyMCEUtilities + (script ?? String.Empty);
        }

        #endregion
    }
}
