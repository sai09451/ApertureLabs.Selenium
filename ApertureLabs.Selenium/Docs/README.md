# README

> A utility library containing definitions for PageObjects/PageComponents and
> provides addtional classes and extension methods when working with Selenium. 

These include:
	* InputElement
		* CheckboxElement
	* IFrame
	* TabHelper
	* TextHelper
	* WebDriverFactory
	* etc...

## Usage

### WebDriverFactory

var webDriverFactory = new WebDriverFactory();
var chrome = webDriverFactory.CreateDriver(MajorDriver.Chrome, WindowSize.DefaultDesktop);

// Do stuff with chrome.

// Optionally dispose chrome.
// chrome.Dispose();

// Or dispose the factory which will dispose all tracked drivers.
webDriverFactory.dispose();

### InputElement

var element = driver.FindElement(By.CssSelector("input"));
var inputEl = new InputElement(element);

string valueAsStr = inputEl.GetValue();
bool valueAsBool = inputEl.GetValue();

### IFrameHelper

var element = driver.FindElement(By.CssSelector("iframe"));
var iframe = new IFrameElement(element);
iframe.Enter();

// Now in iframe.

iframe.Exit();
// Back in parent frame.
