using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.PageObjects;
using ApertureLabs.Selenium.WebElements.Inputs;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ApertureLabs.Selenium.Components.JQuery.DatePicker
{
    public class DatePickerComponent : PageComponent
    {
        #region Fields

        private const string JQueryRequiredClass = "ui-datepicker";

        #region Selectors

        private readonly By CalendarContainerSelector;
        private readonly By CalendarTitleSelector = By.CssSelector(".ui-datepicker-title");
        private readonly By CalendarTitleMonthSelector = By.CssSelector(".ui-datepicker-month");
        private readonly By CalendarTitleYearSelector = By.CssSelector(".ui-datepicker-year");
        private readonly By CalendarNextMonthSelector = By.CssSelector(".ui-datepicker-next");
        private readonly By CalendarPrevMonthSelector = By.CssSelector(".ui-datepicker-prev");
        private readonly By CalendarDaysSelectors = By.CssSelector(".ui-state-default");
        private readonly By CalendarEnabledDaysSelector = By.CssSelector("td:not(.ui-state-disabled) > .ui-state-default");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selector">
        /// The input element linked to the calendar.
        /// </param>
        /// <param name="calendarContainerSelector">
        /// The element that contains the calendar.
        /// </param>
        public DatePickerComponent(IWebDriver driver,
            By selector,
            By calendarContainerSelector)
            : base(driver, selector)
        {
            CalendarContainerSelector = calendarContainerSelector;
        }

        #endregion

        #region Properties

        public DateTime Date
        {
            get
            {
                return InputElement.GetValue(DateTime.Parse);
            }
            set
            {
                DisplayCalendar();

                // Parse the date displayed by the calendar title
                var cmonth = DateTime.ParseExact(
                        CalendarContainerElement
                            .FindElement(CalendarTitleMonthSelector)
                            .Text
                            .Trim(),
                        "MMMM",
                        CultureInfo.CurrentCulture)
                    .Month;
                var cyear = DateTime.ParseExact(
                    WrappedDriver.FindElement(CalendarTitleYearSelector)
                            .Text
                            .Trim(),
                        "YYYY",
                        CultureInfo.CurrentCulture)
                    .Year;

                var calendarDate = new DateTime(cyear, cmonth, 0);

                // This if statement sets the calendar to the correct month/year
                if (value.Year > calendarDate.Year)
                {
                    for (int i = 0; i < (value.Subtract(calendarDate).Days / (365.25 / 12)); i++)
                    {
                        NextMonth();
                    }
                }
                else if (value.Year < calendarDate.Year)
                {
                    for (int i = 0; i < (calendarDate.Subtract(value).Days / (365.25 / 12)); i++)
                    {
                        PreviousMonth();
                    }
                }

                // Then select the day
                SelectDay(value.Day);
            }
        }

        private int ActiveYear
        {
            get
            {
                return DateTime.ParseExact(
                    CalendarContainerElement.FindElement(CalendarTitleYearSelector)
                            .Text
                            .Trim(),
                        "YYYY",
                        CultureInfo.CurrentCulture)
                    .Year;
            }
        }

        private int ActiveMonth
        {
            get
            {
                return DateTime.ParseExact(
                        CalendarContainerElement.FindElement(CalendarTitleMonthSelector)
                            .Text
                            .Trim(),
                        "MMMM",
                        CultureInfo.CurrentCulture)
                    .Month;
            }
        }

        #region Elements

        private IWebElement CalendarContainerElement => WrappedDriver.FindElement(CalendarContainerSelector);
        private InputElement InputElement => new InputElement(WrappedElement);

        #endregion

        #endregion

        #region Functions

        public IEnumerable<DateTime> GetEnabledDateTimes()
        {
            DisplayCalendar();

            int wentBack = 0;
            int wentForward = 0;

            // Navigate to first month
            for (; HasPrevious(); wentBack++)
            {
                if (wentBack == 10)
                {
                    throw new Exception("Exceeded the maximum number of " +
                        "tries to find the enabled range (The starting date " +
                        "range was more than 10 months in the past)");
                }

                PreviousMonth();
            }

            var firstEnabledDay = int.Parse(
                CalendarContainerElement.FindElement(CalendarDaysSelectors)
                    .Text
                    .Trim());

            var firstDate = new DateTime(ActiveYear, ActiveMonth, firstEnabledDay);

            // Navigate back to the month the calendar started on
            for (; wentBack > 0; wentBack--)
                NextMonth();

            // Navigate to last month
            for (; HasNext(); wentForward++)
            {
                if (wentForward == 10)
                {
                    throw new Exception("Exceeded the maximum number of " +
                        "tries to find the enabled range (The ending date " +
                        "rage was more than 10 months in the future)");
                }

                NextMonth();
            }

            var enabledDays = CalendarContainerElement
                .FindElements(CalendarEnabledDaysSelector);

            var lastDate = new DateTime(ActiveYear,
                ActiveMonth,
                int.Parse(enabledDays.Last().Text.Trim()));

            for (int i = 0; i < wentForward; i++)
                PreviousMonth();

            return new DateTime[]
            {
                firstDate,
                lastDate
            };
        }

        private bool HasNext()
        {
            return CalendarContainerElement.FindElement(CalendarNextMonthSelector)
                .Classes()
                .Contains("ui-state-disabled");
        }

        private bool HasPrevious()
        {
            return CalendarContainerElement.FindElement(CalendarPrevMonthSelector)
                .Classes()
                .Contains("ui-state-disabled");
        }

        private void HideCalendar()
        {
            CalendarContainerElement.SendKeys(Keys.Escape);
        }

        private void DisplayCalendar()
        {
            if (!CalendarContainerElement.Displayed)
            {
                CalendarContainerElement.Click();
                WrappedDriver.Wait(TimeSpan.FromSeconds(2))
                    .Until(d => CalendarContainerElement.Displayed);
            }
        }

        private void NextMonth()
        {
            CalendarContainerElement.FindElement(CalendarNextMonthSelector).Click();

            WrappedDriver
                .Wait(TimeSpan.FromMilliseconds(500))
                .Until(d => IsCalendarExpanded());
        }

        private void PreviousMonth()
        {
            CalendarContainerElement.FindElement(CalendarPrevMonthSelector).Click();

            WrappedDriver
                .Wait(TimeSpan.FromMilliseconds(500))
                .Until(d => IsCalendarExpanded());
        }

        private bool TrySelectDay(int dayNumber)
        {
            try
            {
                SelectDay(dayNumber);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private void SelectDay(int dayNumber)
        {
            // Make sure dayNumber is greater than zero
            if (dayNumber <= 0)
            {
                throw new ArgumentOutOfRangeException($"Argument " +
                    $"dayNumber({dayNumber}) cannot be negative nor zero");
            }

            var availableDays = CalendarContainerElement.FindElements(CalendarDaysSelectors);

            // Make sure the dayNumber is less or equal to the max number of
            // days for that month
            if (availableDays.Count < dayNumber)
            {
                throw new ArgumentOutOfRangeException($"Tried to select " +
                    $"day {dayNumber} but only {availableDays.Count} days " +
                    $"were present in the month");
            }

            availableDays[dayNumber - 1].Click();
            WrappedDriver.Wait(TimeSpan.FromMilliseconds(500));
        }

        private bool IsCalendarExpanded()
        {
            return false;
        }

        #endregion
    }
}
