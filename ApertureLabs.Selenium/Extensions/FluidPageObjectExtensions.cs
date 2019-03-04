using ApertureLabs.Selenium.PageObjects;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Events;
using System;

namespace ApertureLabs.Selenium.Extensions
{
    /// <summary>
    /// Contains extensions methods for objects implementing the IPageObject.
    /// interface.
    /// </summary>
    public static class FluidPageObjectExtensions
    {
        /// <summary>
        /// Will load up the page and navigate to it's url.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageObjectFactory">The page object factory.</param>
        /// <param name="performActions">
        /// The actions to be performed on the page. No navigation events
        /// should occur here.
        /// </param>
        /// <returns></returns>
        public static T StartWithPage<T>(
            this IPageObjectFactory pageObjectFactory,
            Action<T> performActions = null)
            where T : IStaticPageObject
        {
            var page = pageObjectFactory.PreparePage<T>();
            page.Load(true);

            if (performActions != null)
            {
                TryAddNavigatedHandler(page.WrappedDriver);
                performActions(page);
                TryRemoveNavigatedHandler(page.WrappedDriver);
            }

            return page;
        }

        /// <summary>
        /// Used to chaing actions between <see cref="IPageObject"/> instances.
        /// The only time the wrapped <see cref="IWebDriver"/> should
        /// navigate is in the navigationActions argument..
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="pageObject">The page object.</param>
        /// <param name="performActions">
        /// The actions on a web-page that don't result in a navigation event.
        /// </param>
        /// <param name="navigationActions">
        /// The actions on a web-page that result in a navigation event. Should
        /// return the fully loaded <see cref="IPageObject"/>.
        /// </param>
        /// <returns>The loaded <see cref="IPageObject"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if performActions or navigationActions are null.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown if a Navigated event is detected while the argument
        /// performing actions is called.
        /// </exception>
        /// <remarks>
        /// Exists for organization. Instead of manually indenting the chained
        /// method calls (if using a fluid syntax) this can be used to better
        /// seperate which methods are acting on which pages.
        /// </remarks>
        public static V ContinueWithPage<T, V>(this T pageObject,
            Action<T> performActions,
            Func<T, V> navigationActions)
            where T : IPageObject
            where V : IPageObject
        {
            if (navigationActions == null)
                throw new ArgumentNullException(nameof(navigationActions));
            else if (performActions == null)
                throw new ArgumentNullException(nameof(performActions));

            TryAddNavigatedHandler(pageObject.WrappedDriver);
            performActions(pageObject);
            TryRemoveNavigatedHandler(pageObject.WrappedDriver);

            return navigationActions(pageObject);
        }

        /// <summary>
        /// Used to chaing actions between <see cref="IPageObject"/> instances.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="pageObject">The page object.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <returns>
        /// The newly loaded <see cref="IPageObject"/> instance.
        /// </returns>
        /// <remarks>
        /// Exists for organization. Instead of manually indenting the chained
        /// method calls (if using a fluid syntax) this can be used to better
        /// seperate which methods are acting on which pages.
        /// </remarks>
        public static V ContinueWithPage<T, V>(this T pageObject,
            Func<T, V> navigationActions)
            where T : IPageObject
            where V : IPageObject
        {
            return navigationActions(pageObject);
        }

        /// <summary>
        /// Used as the final link in a chain of method calls between
        /// <see cref="IPageObject"/> instances. The wrapped
        /// <see cref="IWebDriver"/> should never navigate to another url in
        /// this method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageObject">The page object.</param>
        /// <param name="performAction">
        /// The final action(s) to be performed on a <see cref="IPageObject"/>.
        /// </param>
        /// <exception cref="Exception">
        /// Thrown if a navigation event is detected while performingAction is
        /// executing.
        /// </exception>
        /// <remarks>
        /// Exists for organization. Instead of manually indenting the chained
        /// method calls (if using a fluid syntax) this can be used to better
        /// seperate which methods are acting on which pages.
        /// </remarks>
        public static void ContinueWithPage<T>(this T pageObject,
            Action<T> performAction)
            where T : IPageObject
        {
            TryAddNavigatedHandler(pageObject.WrappedDriver);
            performAction(pageObject);
            TryRemoveNavigatedHandler(pageObject.WrappedDriver);
        }

        private static void TryAddNavigatedHandler(IWebDriver driver)
        {
            if (driver is EventFiringWebDriver eventFiringWebDriver)
                eventFiringWebDriver.Navigated += EventFiringWebDriver_Navigated;
        }

        private static void TryRemoveNavigatedHandler(IWebDriver driver)
        {
            if (driver is EventFiringWebDriver eventFiringWebDriver)
                eventFiringWebDriver.Navigated -= EventFiringWebDriver_Navigated;
        }

        private static void EventFiringWebDriver_Navigated(object sender,
            WebDriverNavigationEventArgs e)
        {
            throw new Exception("Detected a navigation event.");
        }
    }
}
