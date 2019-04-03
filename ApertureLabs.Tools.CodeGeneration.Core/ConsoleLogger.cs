using ApertureLabs.Tools.CodeGeneration.Core.Options;
using Colorful;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Console = Colorful.Console;

namespace ApertureLabs.Tools.CodeGeneration.Core
{
    public class ConsoleLogger
    {
        #region Fields

        private const string MESSAGE_FORMAT = "[{0}] - {1} : {2}";
        private readonly object lockContext = new object();
        private readonly LogOptions logOptions;

        #endregion

        #region Constructor

        public ConsoleLogger(LogOptions logOptions)
        {
            this.logOptions = logOptions
                ?? throw new ArgumentNullException(nameof(logOptions));
        }

        #endregion

        #region Methods

        public void Info(object message)
        {
            Log(
                message: message,
                logLevel: "INFO",
                fg: Color.White);
        }

        public void Warning(object message)
        {
            Log(
                message: message,
                logLevel: "WARNING",
                fg: Color.Yellow);
        }

        public void Debug(object message)
        {
            Log(
                message: message,
                logLevel: "DEBUG",
                fg: Color.Purple);
        }

        public void Error(object message, bool @throw = false)
        {
            Log(
                message: message,
                logLevel: "DEBUG",
                fg: Color.Red);

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
                    Console.WriteLine(msg);
                else
                    Console.WriteLine(msg, fg);
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

        private static string FormatMessage(string messageLevel, object message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var messageStr = message as string ?? message.ToString();

            return String.Format(
                CultureInfo.CurrentCulture,
                MESSAGE_FORMAT,
                DateTime.UtcNow,
                messageLevel,
                messageStr);
        }

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

                    Console.Write(
                        prefix.PadRight(Console.BufferWidth),
                        Color.White);

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

                    Console.Write(sb.ToString(), Color.White);

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
            private readonly Color fg;
            private readonly Color bg;

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
