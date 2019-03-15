using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ApertureLabs.Selenium.Extensions;
using ApertureLabs.Selenium.Properties;
using ApertureLabs.Selenium.WebElements.Inputs;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ApertureLabs.Selenium.Components.JQuery.DatePicker
{
    /// <summary>
    /// DatePickerComponent.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="JQueryWidgetBase{T}" />
    public class DatePickerComponent<T> : JQueryWidgetBase<T>
    {
        #region Fields

        private readonly DatePickerComponentOptions datePickerComponentOptions;

        #region Selectors

        private readonly By calendarContainerSelector = By.CssSelector(".ui-datepicker");
        private readonly By titleSelector = By.CssSelector(".ui-datepicker-title");
        private readonly By monthTitleSelector = By.CssSelector(".ui-datepicker-month");
        private readonly By yearTitleSelector = By.CssSelector(".ui-datepicker-year");
        private readonly By allDaysSelector = By.CssSelector(".ui-datepicker-calendar tbody td a");
        private readonly By availabledaysSelector = By.CssSelector(".ui-datepicker-calendar tbody td:not(.ui-datepicker-unselectable) a");
        private readonly By activeDaySelector = By.CssSelector(".ui-datepicker-current-day a");
        private readonly By calendarNextMonthSelector = By.CssSelector(".ui-datepicker-next");
        private readonly By calendarPrevMonthSelector = By.CssSelector(".ui-datepicker-prev");
        private readonly By calendarDaysSelectors = By.CssSelector(".ui-state-default");
        private readonly By calendarEnabledDaysSelector = By.CssSelector("td:not(.ui-state-disabled) > .ui-state-default");

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="DatePickerComponent{T}"/> class.
        /// </summary>
        /// <param name="selector">
        /// The input element linked to the calendar.
        /// </param>
        /// <param name="datePickerComponentOptions">
        /// The datepciker component options.
        /// </param>
        /// <param name="driver">The driver.</param>
        /// <param name="parent">The parent.</param>
        public DatePickerComponent(By selector,
            DatePickerComponentOptions datePickerComponentOptions,
            IWebDriver driver,
            T parent)
            : base(selector, driver, parent)
        {
            this.datePickerComponentOptions = datePickerComponentOptions;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is popup.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is popup; otherwise, <c>false</c>.
        /// </value>
        protected bool IsPopup { get; set; }

        #region Elements

        private InputElement InputElement => IsPopup ? new InputElement(WrappedElement) : null;
        private IWebElement MonthTitleElement => CalendarContainerElement.FindElement(monthTitleSelector);
        private IWebElement YearTitleElement => CalendarContainerElement.FindElement(yearTitleSelector);
        private IWebElement ActiveDayElement => CalendarContainerElement.FindElements(activeDaySelector).FirstOrDefault();
        private IReadOnlyCollection<IWebElement> AllDayElements => CalendarContainerElement.FindElements(allDaysSelector);

        private IWebElement CalendarContainerElement
        {
            get
            {
                IWebElement container;

                if (IsPopup)
                {
                    var script =
                        "var el = arguments[0];" +
                        "return $(el).data().datepicker.dpDiv[0];";

                    container = (IWebElement)WrappedDriver
                        .JavaScriptExecutor()
                        .ExecuteScript(script, WrappedElement);
                }
                else
                {
                    container = WrappedElement
                        .FindElement(calendarContainerSelector);
                }

                return container;
            }
        }

        #endregion

        #endregion

        #region Functions

        /// <summary>
        /// If overriding don't forget to call base.Load() or make sure to
        /// assign the WrappedElement.
        /// </summary>
        /// <returns></returns>
        public override ILoadableComponent Load()
        {
            base.Load();

            // Check if the WrappedElement is an input, if it is the
            // datepicker container will be appended in the body.
            IsPopup = WrappedElement.TagName == "input";

            return this;
        }

        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <returns></returns>
        public DateTime? GetDate()
        {
            if (IsPopup)
            {
                // Return null if no date has been set yet.
                if (String.IsNullOrEmpty(InputElement?.GetValue<string>()))
                    return null;

                var date = InputElement.GetValue(
                    value => DateTime.ParseExact(
                        value,
                        datePickerComponentOptions.DateFormat,
                        CultureInfo.InvariantCulture));

                return date;
            }
            else
            {
                var year = GetYear();
                var month = GetMonth();
                var day = GetDay();
                day = day == -1 ? 1 : day;

                return new DateTime(year, month, day);
            }
        }

        /// <summary>
        /// Sets the date.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        public void SetDate(DateTime dateTime)
        {
            // Display the calendar if popup.
            DisplayCalendar();

            // Set the year/month.
            var currentYear = GetYear();
            var currentMonth = GetMonth();
            var currentDay = GetDay();
            currentDay = currentDay == -1 ? 1 : currentDay;

            var currentDate = new DateTime(
                currentYear,
                currentMonth,
                currentDay);

            var monthsDiff = DifferenceInMonths(currentDate, dateTime);
            var isPositive = monthsDiff > 0;

            monthsDiff = Math.Abs(monthsDiff);

            for (var i = 0; i < monthsDiff; i++)
            {
                if (isPositive)
                    PreviousMonth();
                else
                    NextMonth();
            }

            // Set the day.
            SelectDay(dateTime.Day);

            HideCalendar();
        }

        /// <summary>
        /// Determines whether this instance is disabled.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is disabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDisabled()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the min and max date times.
        /// </summary>
        /// <returns></returns>
        public (DateTime? minDate, DateTime? maxDate) GetMinAndMaxDates()
        {
            var options = GetOptions();

            if (options.TryGetValue("minDate", out var minDateToken)
                && options.TryGetValue("maxDate", out var maxDateToken))
            {
                var minDate = minDateToken.ToObject<DateTime?>();
                var maxDate = maxDateToken.ToObject<DateTime?>();

                return (minDate, maxDate);
            }
            else
            {
                throw new Exception("All dates enabled.");
            }
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <returns></returns>
        protected JObject GetOptions()
        {
            var script =
                Resources.jsonFormatterCallback +
                "var optNames = [" +
                    "altField," +
                    "altFormat," +
                    "appendText," +
                    "autoSize," +
                    "beforeShow," +
                    "beforeShowDay," +
                    "buttonImage," +
                    "buttonImageOnly," +
                    "buttonText," +
                    "calculateWeek," +
                    "changeMonth," +
                    "changeYear," +
                    "closeText," +
                    "constrainInput," +
                    "currentText," +
                    "dateFormat," +
                    "dayNames," +
                    "dayNamesMin," +
                    "dayNamesShort," +
                    "defaultDate," +
                    "duration," +
                    "firstDay," +
                    "gotoCurrent," +
                    "hideIfNoPrevNext," +
                    "isRTL," +
                    "maxDate," +
                    "minDate," +
                    "monthNames," +
                    "monthNamesShort," +
                    "navigationAsDateFormat," +
                    "nextText," +
                    "numberOfMonths," +
                    "onChangeMonthYear," +
                    "onClose," +
                    "onSelect," +
                    "prevText," +
                    "selectOtherMonths," +
                    "shortYearCutoff," +
                    "showAnim," +
                    "showButtonPanel," +
                    "showCurrentAtPos," +
                    "showMonthAfterYear," +
                    "showOn," +
                    "showOptions," +
                    "showOtherMonths," +
                    "showWeek," +
                    "stepMonths," +
                    "weekHeader," +
                    "yearRange," +
                    "yearSuffix" +
                "];" +
                "var el = arguments[0];" +
                "var opt = { };" +
                "for (var i = 0; i < optNames.length; i++ {" +
                    "var optName = optNames[i];" +
                    "var val = $(el).data().datapicker('option', optNames[i]); " +
                    "opt[optName] = val;" +
                "}" +
                "return JSON.stringify(opt, jsonFormatter);";

            var optionsStr = (string)WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);

            return JObject.Parse(optionsStr);
        }

        /// <summary>
        /// Gets the data object.
        /// </summary>
        /// <returns></returns>
        protected JObject GetData()
        {
            var script =
                Resources.jsonFormatterCallback +
                "var el = arguments[0];" +
                "var dataObj = $(el).data().datepicker;" +
                "return JSON.stringify(dataObj, jsonFormatter);";

            var dataStr = (string)WrappedDriver
                .JavaScriptExecutor()
                .ExecuteScript(script, WrappedElement);

            return JObject.Parse(dataStr);
        }

        private int GetYear()
        {
            return YearTitleElement.TextHelper().ExtractInteger();
        }

        private int GetMonth()
        {
            return DateTime.ParseExact(
                    MonthTitleElement.TextHelper().InnerText,
                    "MMMM",
                    CultureInfo.CurrentCulture)
                .Month;
        }

        /// <summary>
        /// Gets the day. Returns -1 if no day has been selected.
        /// </summary>
        /// <returns></returns>
        private int GetDay()
        {
            return ActiveDayElement?.TextHelper().ExtractInteger() ?? -1;
        }

        private bool HasNext()
        {
            return CalendarContainerElement.FindElement(calendarNextMonthSelector)
                .Classes()
                .Contains("ui-state-disabled");
        }

        private bool HasPrevious()
        {
            return CalendarContainerElement.FindElement(calendarPrevMonthSelector)
                .Classes()
                .Contains("ui-state-disabled");
        }

        private void NextMonth()
        {
            CalendarContainerElement.FindElement(calendarNextMonthSelector).Click();

            WrappedDriver
                .Wait(TimeSpan.FromMilliseconds(500))
                .Until(d => CalendarContainerElement.Displayed);
        }

        private void PreviousMonth()
        {
            CalendarContainerElement.FindElement(calendarPrevMonthSelector).Click();

            WrappedDriver
                .Wait(TimeSpan.FromMilliseconds(500))
                .Until(d => CalendarContainerElement.Displayed);
        }

        private int DifferenceInMonths(DateTime date1, DateTime date2)
        {
            var diff = ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;

            return diff;
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

            var availableDays = CalendarContainerElement.FindElements(calendarDaysSelectors);

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

        private void DisplayCalendar()
        {
            if (IsPopup && !CalendarContainerElement.Displayed)
            {
                WrappedElement.Click();

                WrappedDriver.Wait(datePickerComponentOptions.AnimationDuration)
                    .Until(d => CalendarContainerElement.Displayed);
            }
        }

        private void HideCalendar()
        {
            if (IsPopup && CalendarContainerElement.Displayed)
            {
                WrappedDriver.CreateActions()
                    .SendKeys(Keys.Escape)
                    .Perform();
                WrappedDriver.Wait(datePickerComponentOptions.AnimationDuration)
                    .Until(d => !CalendarContainerElement.Displayed);
            }
        }

        #endregion
    }
}
