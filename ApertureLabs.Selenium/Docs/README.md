# README

> A utility library containing definitions for PageObjects/PageComponents and
> provides addtional classes/extension methods for working with Selenium. 

These include:
* InputElement
* CheckboxElement
* IFrame
* TabHelper
* TextHelper
* PageObjectFactory
* WebDriverFactory
* etc...

## Goals
* Remain consistent and compatible with the patterns used by Selenium.
* Standardize the way PageObjects are written.
* Simplify the way WebDrivers are managed.
* Provide polyfills for selenium to allow more consistent behaviour.
* Provide common utilities for working with selectors, css, js.

### WebDriverFactory
No need to download the needed webdrivers, this class will do if for you!
Supports both hub+node or standalone configuration depending on your situation.

Example:

```
// Defaults to standalone mode.
var webDriverFactory = new WebDriverFactory();

// This will download the latest driver for chrome, initialize it, and
// set the window size.
var chrome = webDriverFactory.CreateDriver(
	MajorDriver.Chrome,
	WindowSize.DefaultDesktop);

// Do stuff with chrome.
chrome.Navigate().GoToUrl("https://google.com");

// Optionally dispose chrome.
// chrome.Dispose();

// Or dispose the factory which will dispose all tracked drivers.
webDriverFactory.dispose();
```

### BaseElement
A common class which all ApertureLabs.Selenium custom elements/element wrappers
inherit from. It provides default implementations of IWebElement and the
explicitly implemented interfaces of RemoteWebElement (that way casting it as
an IWrapsDriver still works). Its recommended to inherit this class for all
custom elements/element wrappers. This will ensure that all extensions methods
(such as *yourCustomEl*.GetDriver()) that rely on casting will still work.

### InputElement
This is a minor wrapper which simplifies setting and getting the value of an
input element. CheckboxElement, InputDataListElement, and other input related
elements all derive from this class. NOTE: When calling
`inputEl.SetValue("your value");` this will CLEAR the previous value before
entering the new one. If you want to append to the value use
`inputEl.SendKeys("your value");` instead.

Example:
```
var element = driver.FindElement(By.CssSelector("input"));
var inputEl = new InputElement(element);

string valueAsStr = inputEl.GetValue();
bool valueAsBool = inputEl.GetValue();
```

### IFrameHelper
Example:
```
var element = driver.FindElement(By.CssSelector("iframe"));
var iframe = new IFrameElement(element);
iframe.Enter();

// Now in iframe.

iframe.Exit();
// Back in parent frame.
```

### PageObjects
Includes interfaces and default implementations of interfaces:
* IPageObject -> PageObject
  * Used for creating classes that represent web pages.
* IPageComponent -> PageComponent
  * Used for creating reusable components.
* IFluidPageComponent -> FluidPageComponent
  * Used for creating 'fluid' (aka chainable) components.
* IPageObjectFactory -> PageObjectFactory
  * Used for preparing/loading IPageObjects and IPageComponents.
  * Uses dependency injection to create IPageObjects.