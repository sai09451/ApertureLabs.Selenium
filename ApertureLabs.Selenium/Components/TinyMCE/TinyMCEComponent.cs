using System;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
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

        private IntegrationMode integrationMode;
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

        private IWebElement SourceElement => integrationMode == IntegrationMode.Classic
            ? WrappedElement
            : WrappedDriver.FindElement(By.TagName("html"));

        private IWebElement EditableBodyElement => SourceElement.FindElement(editableBodySelector);

        #endregion

        #endregion

        #region Methods

        /// <inheritDoc/>
        public override ILoadableComponent Load()
        {
            UpdateIntegrationMode();

            if (integrationMode == IntegrationMode.Classic)
            {
                 iframeElement = new IFrameElement(
                     WrappedElement.FindElement(By.TagName("iframe")),
                     WrappedDriver);
            }

            // Menu.
            if (SourceElement.FindElements(menuComponentSelector).Any())
            {
                menu = new MenuComponent(WrappedDriver,
                    menuComponentSelector);
            }
            else
            {
                menu = null;
            }

            // Toolbar.
            if (SourceElement.FindElements(toolbarComponentSelector).Any())
            {
                toolbar = new ToolbarComponent(WrappedDriver,
                    toolbarComponentSelector);
            }
            else
            {
                toolbar = null;
            }

            // Status bar.
            if (SourceElement.FindElements(statusbarComponentSelector).Any())
            {
                statusbar = new StatusbarComponent(WrappedDriver,
                    statusbarComponentSelector);
            }
            else
            {
                statusbar = null;
            }

            WrappedElement = SourceElement;

            return this;
        }

        /// <summary>
        /// Writes the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void Write(string content)
        {
            if (integrationMode == IntegrationMode.Classic)
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
        /// Clears all content in the editable body area.
        /// </summary>
        public void ClearAllContent()
        {
            if (integrationMode == IntegrationMode.Classic)
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
                integrationMode = IntegrationMode.Classic;
            }
            else
            {
                // Could be either classic or distraction free.
                const string script =
                    "var el = arguments[0]" +
                    "for (var i = 0; i < tinyMCE.editors.length; i++) {" +
                        "var editor = tinyMCE.editors[i];" +
                        "var bodyEl = editor.getElement();" +
                        "if (bodyEl == el) {" +
                            "return bodyEl.getParam('theme') === 'inlite';" +
                        "}" +
                    "}" +
                    "return false;";

                var isDistractionFreeMode = (bool)WrappedDriver
                    .JavaScriptExecutor()
                    .ExecuteScript(script, WrappedElement);

                integrationMode = isDistractionFreeMode
                    ? IntegrationMode.DistractionFree
                    : IntegrationMode.Inline;
            }
        }

        #endregion
    }
}
