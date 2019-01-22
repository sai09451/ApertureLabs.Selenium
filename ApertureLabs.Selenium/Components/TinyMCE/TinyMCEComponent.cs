using System;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Js;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.Properties;
using ApertureLabs.Selenium.WebElements.IFrame;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.TinyMCE
{
    /// <summary>
    /// Used for interacting with TinyMCE editors.
    /// </summary>
    /// <seealso cref="ApertureLabs.Selenium.PageObjects.PageComponent" />
    public class TinyMCEComponent : PageComponent
    {
        #region Fields

        private readonly IPageObjectFactory pageObjectFactory;
        private MenuComponent menu;
        private ToolbarComponent toolbar;
        private StatusbarComponent statusbar;
        private IFrameElement iframeElement;

        #region Selectors

        private readonly By menuComponentSelector = By.CssSelector("*[role='menubar']");
        private readonly By toolbarComponentSelector = By.CssSelector("*[role='toolbar']");
        private readonly By statusbarComponentSelector = By.CssSelector(".mce-statusbar");
        private readonly By editableBodySelector = By.CssSelector(".mce-edit-area");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TinyMCEComponent"/> class.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="pageObjectFactory"></param>
        /// <param name="driver">The driver.</param>
        public TinyMCEComponent(By selector,
            IPageObjectFactory pageObjectFactory,
            IWebDriver driver)
            : base(driver, selector)
        {
            this.pageObjectFactory = pageObjectFactory;
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
        public MenuComponent Menu
        {
            get
            {
                if (menu != null)
                    return pageObjectFactory.PrepareComponent(menu);
                else
                    return null;
            }
        }

        /// <summary>
        /// Used to manipulate the toolbar. Null if no toolbar.
        /// </summary>
        public ToolbarComponent Toolbar
        {
            get
            {
                if (toolbar != null)
                    return pageObjectFactory.PrepareComponent(toolbar);
                else
                    return null;
            }
        }

        /// <summary>
        /// Used to manipulate the statusbar. Null if no status bar.
        /// </summary>
        public StatusbarComponent Statusbar
        {
            get
            {
                if (statusbar != null)
                    return pageObjectFactory.PrepareComponent(statusbar);
                else
                    return null;
            }
        }

        #region Elements

        /// <summary>
        /// Gets or sets the element created by tinyMCE.
        /// </summary>
        private IWebElement TinyMCECreatedElement { get; set; }

        private IWebElement EditableBodyElement => IntegrationMode == IntegrationMode.Classic
            ? WrappedDriver.FindElement(By.TagName("body"))
            : TinyMCECreatedElement.FindElement(editableBodySelector);

        #endregion

        #endregion

        #region Methods

        /// <inheritDoc/>
        public override ILoadableComponent Load()
        {
            base.Load();

            WaitForInitalization();

            var script =
                "var el = arguments[0];" +
                "var editor = tinyMCEUtilties.getEditor(el);" +
                "return editor == null ? null : editor.getContent();";

            script = AddTinyMCEUtilities(script);
            script = JavaScript.TrimWhitespace(script);

            TinyMCECreatedElement = (IWebElement)WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);

            UpdateIntegrationMode();

            if (IntegrationMode == IntegrationMode.Classic)
            {
                 iframeElement = new IFrameElement(
                     WrappedElement.FindElement(By.TagName("iframe")),
                     WrappedDriver);
            }

            // Menu.
            if (TinyMCECreatedElement.FindElements(menuComponentSelector).Any())
            {
                menu = new MenuComponent(menuComponentSelector,
                    pageObjectFactory,
                    WrappedDriver);
            }
            else
            {
                menu = null;
            }

            // Toolbar.
            if (TinyMCECreatedElement.FindElements(toolbarComponentSelector).Any())
            {
                toolbar = new ToolbarComponent(WrappedDriver,
                    toolbarComponentSelector);
            }
            else
            {
                toolbar = null;
            }

            // Status bar.
            if (TinyMCECreatedElement.FindElements(statusbarComponentSelector).Any())
            {
                statusbar = new StatusbarComponent(WrappedDriver,
                    statusbarComponentSelector);
            }
            else
            {
                statusbar = null;
            }

            return this;
        }

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void Write(string content)
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
        public void WriteLine(string content)
        {
            Write(content);
            Write(Keys.Enter);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        public void WriteLine()
        {
            Write(Keys.Enter);
        }

        /// <summary>
        /// Gets the content of the editor. This calls the
        /// tinyMCE.Editor.getContent().
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            var script =
                "var el = arguments[0];" +
                "var editor = tinyMCEUtilties.getEditor(el);" +
                "return editor == null ? null : editor.getContent();";

            script = AddTinyMCEUtilities(script);
            script = JavaScript.TrimWhitespace(script);

            return (string)WrappedDriver.JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);
        }

        /// <summary>
        /// Clears all content in the editable body area.
        /// </summary>
        public void ClearAllContent()
        {
            if (IntegrationMode == IntegrationMode.Classic)
                iframeElement.InFrameAction(_ClearAllContent);
            else
                _ClearAllContent();
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
            if (WrappedElement.FindElements(By.TagName("iframe")).Any())
            {
                // Only classic mode uses iframes.
                IntegrationMode = IntegrationMode.Classic;
            }
            else
            {
                // Could be either classic or distraction free.
                var script =
                    "var el = arguments[0];" +
                    "var editor = tinyMCEUtilties.getEditor(el);" +
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
                "var el = {2}[0];" +
                "var editor = tinyMCEUtilities.getEditor(el);" +
                "tinyMCEUtilities.waitForInitialization(editor," +
                    timeoutMS +
                    pollingMS +
                    "{0}," +
                    "{1});";

            script = AddTinyMCEUtilities(script);
            script = JavaScript.EscapeScript(script);

            var waiter = new PromiseBody(script);

            wait.Until(d => waiter.Finished);
            waiter.Wait(TimeSpan.FromSeconds(10));
        }

        private string AddTinyMCEUtilities(string script)
        {
            return Resources.tinyMCEUtilities + script;
        }

        #endregion
    }
}
