using ApertureLabs.Tools.CodeGeneration.Core.Options;
using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using Console = System.Console;
//using Console = Colorful.Console;

namespace ApertureLabs.Tools.CodeGeneration.Core
{
    public class ConsoleLogger
    {
        #region Fields

        private const string MESSAGE_FORMAT = "[{0}] - {1} : {2}";
        private readonly object lockContext = new object();
        private readonly LogOptions logOptions;
        private readonly Color infoColor;
        private readonly Color warningColor;
        private readonly Color debugColor;
        private readonly Color errorColor;

        #endregion

        #region Constructor

        public ConsoleLogger(LogOptions logOptions)
        {
            this.logOptions = logOptions
                ?? throw new ArgumentNullException(nameof(logOptions));

            //infoColor = GetConstrastColor(Color.White, Color.Black);
            infoColor = Color.LightGray;
            //warningColor = GetConstrastColor(Color.Yellow, Color.DarkKhaki);
            warningColor = Color.Yellow;
            //debugColor = GetConstrastColor(Color.Purple, Color.DarkMagenta);
            debugColor = Color.Purple;
            //errorColor = GetConstrastColor(Color.Red, Color.Pink);
            errorColor = Color.Red;
        }

        #endregion

        #region Methods

        public void Info(object message)
        {
            Log(
                message: message,
                logLevel: "INFO",
                fg: infoColor);
        }

        public void Warning(object message)
        {
            Log(
                message: message,
                logLevel: "WARNING",
                fg: warningColor);
        }

        public void Debug(object message)
        {
            Log(
                message: message,
                logLevel: "DEBUG",
                fg: debugColor);
        }

        public void Error(object message, bool @throw = false)
        {
            Log(
                message: message,
                logLevel: "Error",
                fg: errorColor);

            if (@throw)
            {
                if (message is Exception exc)
                    throw exc;
                else
                    throw new Exception(message.ToString());
            }
        }

        public void Log(object message,
            string logLevel = "INFO",
            Color? fg = null)
        {
            lock (lockContext)
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                // Make sure loglevel is uppercase.
                logLevel = logLevel.ToUpper(CultureInfo.CurrentCulture);

                var msg = FormatMessage(logLevel, message);

                if (logOptions.NoColor)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    var oldFg = Console.ForegroundColor;
                    Console.ForegroundColor = (fg ?? infoColor).ToNearestConsoleColor();
                    Console.WriteLine(msg);
                    Console.ForegroundColor = oldFg;
                }
            }
        }

        public IProgress<T> DeterminateProgressBar<T>(
            Func<T, double> converter,
            string name = null)
        {
            if (logOptions.StructuredOutput)
                return new Progress<T>();

            if (Console.CursorLeft != 0)
            {
                throw new Exception("Expected the cursor to be at the far " +
                    "left position.");
            }

            var progressBar = new _DeterminateProgressBar<T>(
                converter,
                lockContext,
                name);

            return progressBar.Progress;
        }

        public IProgress<T> IndeterminateProgressBar<T>(
            Func<T, string> converter,
            string name = null)
        {
            if (logOptions.StructuredOutput)
                return new Progress<T>();

            if (Console.CursorLeft != 0)
            {
                throw new Exception("Expected the cursor to be at the far " +
                    "left position.");
            }

            var progressBar = new _IndeterminateProgressBar<T>(
                converter,
                lockContext,
                name);

            return progressBar.Progress;
        }

        private static void RecurseThruExecptions(
            Exception exception,
            StringBuilder sb)
        {
            if (exception.InnerException != null)
                RecurseThruExecptions(exception.InnerException, sb);

            sb.AppendJoin(Environment.NewLine,
                Environment.NewLine,
                exception.Message,
                exception.Source,
                exception.StackTrace);
        }

        private static string FormatMessage(string messageLevel, object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            string messageStr;

            if (message is Exception messageExc)
            {
                var sb = new StringBuilder();
                RecurseThruExecptions(messageExc, sb);
                messageStr = sb.ToString();
            }
            else
            {
                messageStr = message as string ?? message.ToString();
            }

            return String.Format(
                CultureInfo.CurrentCulture,
                MESSAGE_FORMAT,
                DateTime.UtcNow,
                messageLevel,
                messageStr);
        }

        //private static Color GetConstrastColor(params Color[] desiredColors)
        //{
        //    var maxL = desiredColors
        //        .Select((color, index) => new { L = GetRelativeLuminance(color), Index = index })
        //        .OrderBy(q => q.L)
        //        .First();

        //    return desiredColors[maxL.Index];
        //}

        //private static double GetRelativeLuminance(Color color)
        //{
        //    Color lighterColor, darkerColor;

        //    if (color.GetBrightness() > Console.BackgroundColor.GetBrightness())
        //    {
        //        lighterColor = color;
        //        darkerColor = Console.BackgroundColor;
        //    }
        //    else
        //    {
        //        darkerColor = color;
        //        lighterColor = Console.BackgroundColor;
        //    }

        //    double
        //        s1 = lighterColor.GetSaturation(),
        //        b1 = lighterColor.GetBrightness(),
        //        s2 = darkerColor.GetSaturation(),
        //        b2 = darkerColor.GetBrightness();

        //    var l1 = (2 - s1) * b1 / 2;
        //    var l2 = (2 - s2) * b2 / 2;

        //    var contrastRatio = (l1 + .05) / (l2 + 0.05);

        //    return contrastRatio;
        //}

        #endregion

        #region Nested Classes

        private class _IndeterminateProgressBar<T>
        {
            public _IndeterminateProgressBar(
                Func<T, string> converter,
                object lockContext,
                string name = null)
            {
                Converter = converter;
                LineNumber = Console.CursorTop;
                Name = name;
                Progress = new Progress<T>(Update);
                LockContext = lockContext;

                lock (lockContext)
                {
                    if (String.IsNullOrEmpty(Name))
                        Console.WriteLine();
                    else
                        Console.WriteLine($"[ {Name} ]");
                }
            }

            public object LockContext { get; }

            public Func<T, string> Converter { get; }

            public int LineNumber { get; }

            public string Name { get; }

            public IProgress<T> Progress { get; }

            public void Done()
            {

            }

            private void Update(T data)
            {
                lock (LockContext)
                {
                    var message = Converter(data);
                    var restorePoint = new ConsoleSettings();
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, LineNumber);
                    var prefix = String.Empty;

                    if (!String.IsNullOrEmpty(Name))
                        prefix += $"[ {Name} ] ";

                    prefix += $"{message}";

                    Console.WriteLine(
                        prefix.PadRight(Console.BufferWidth));

                    // Restore defaults.
                    restorePoint.Restore(true);
                }
            }
        }

        private class _DeterminateProgressBar<T>
        {
            public _DeterminateProgressBar(Func<T, double> converter,
                object lockContext,
                string name = null)
            {
                Converter = converter;
                LineNumber = Console.CursorTop;
                Name = name;
                Progress = new Progress<T>(Update);
                LockContext = lockContext;

                lock (LockContext)
                {
                    if (String.IsNullOrEmpty(Name))
                        Console.WriteLine();
                    else
                        Console.WriteLine($"[ {Name} ]");
                }
            }

            public object LockContext { get; }

            public Func<T, double> Converter { get; }

            public int LineNumber { get; }

            public string Name { get; }

            public IProgress<T> Progress { get; }

            private void Update(T data)
            {
                lock (LockContext)
                {
                    var progress = Converter(data);
                    var restorePoint = new ConsoleSettings();
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(0, LineNumber);
                    var bfWidth = Console.BufferWidth;
                    StringBuilder sb = new StringBuilder(bfWidth);

                    if (!String.IsNullOrEmpty(Name))
                        sb.Insert(0, $"[ {Name} ] ");

                    var progressStr = $" {progress.ToString("0.##", CultureInfo.CurrentCulture)}%";
                    var loadingBarWidth = bfWidth
                        - sb.Length
                        - progressStr.Length;

                    var cells = Convert.ToInt32(
                        Math.Ceiling(
                            loadingBarWidth * (progress / 100)));
                    sb.Insert(0, "=", cells);
                    sb.Append(progressStr);

                    Console.WriteLine(sb.ToString());

                    // Restore defaults.
                    restorePoint.Restore(true);
                }
            }
        }

        private class ConsoleSettings
        {
            private readonly int cursorTop;
            private readonly int cursorLeft;
            private readonly bool cursorVisibility;
            //private readonly Color fg;
            //private readonly Color bg;
            private readonly ConsoleColor fg;
            private readonly ConsoleColor bg;

            public ConsoleSettings()
            {
                cursorTop = Console.CursorTop;
                cursorLeft = Console.CursorLeft;
                cursorVisibility = Console.CursorVisible;
                fg = Console.ForegroundColor;
                bg = Console.BackgroundColor;
            }

            public void Restore(bool position)
            {
                if (position)
                    Console.SetCursorPosition(cursorLeft, cursorTop);

                Console.ForegroundColor = fg;
                Console.BackgroundColor = bg;
                Console.CursorVisible = cursorVisibility;
            }
        }

        #endregion
    }
}
