using System;
using System.Collections.Generic;
using System.Drawing;

namespace ApertureLabs.Selenium.PageObjects
{
    public struct Range
    {
        public int Start;
        public int End;
    }

    public class WindowSize : IComparable
    {
        #region Constructors

        public WindowSize(int minWidth, int? maxWidth = null, int? minHeight = null, int? maxHeight = null)
        {
            MinHeight = minHeight ?? -1;
            MaxHeight = maxHeight ?? -1;
            MinWidth = minWidth;
            MaxWidth = maxWidth ?? -1;

            if (MinHeight < -1
                || MaxHeight < -1
                || MinWidth < -1
                || MaxWidth < -1)
            {
                throw new Exception("Must use positive values for window sizes or -1");
            }

            if (minHeight > maxHeight)
            {
                throw new ArgumentOutOfRangeException("The minHeight({minHeight}) cannot be greater than maxHeight({maxHeight})");
            }

            if (minWidth > maxWidth)
            {
                throw new ArgumentOutOfRangeException("The minHeight({minHeight}) cannot be greater than maxHeight({maxHeight})");
            }
        }

        public WindowSize(int? minWidth, int? maxWidth)
        {
            // Check that none of the arguments are negative
            if (minWidth <= 0 || maxWidth <= 0)
            {
                throw new ArgumentOutOfRangeException("Arugments cannot be negative!");
            }

            // Check that width is in range
            if (minWidth != null || maxWidth != null)
            {
                if (minWidth > maxWidth)
                {
                    throw new ArgumentOutOfRangeException("The minHeight({minHeight}) cannot be greater than maxHeight({maxHeight})");
                }
            }

            // Check that width is in range
            if (minWidth != null || maxWidth != null)
            {
                if (minWidth > maxWidth)
                {
                    throw new ArgumentOutOfRangeException("The minHeight({minHeight}) cannot be greater than maxHeight({maxHeight})");
                }
            }

            MinHeight = -1;
            MaxHeight = -1;
            MinWidth = minWidth ?? -1;
            MaxWidth = maxWidth ?? -1;
        }

        #endregion

        #region Fields

        public static WindowSize DefaultDesktopSize = new WindowSize(1001);
        public static WindowSize DefaultTabletSize = new WindowSize(768, 999);
        public static WindowSize DefaultMobileSize = new WindowSize(null, 767);

        private uint _MaxHeight;
        private uint _MaxWidth;
        private uint _MinHeight;
        private uint _MinWidth;

        #endregion

        #region Properties

        public int MaxHeight
        {
            get => (int)_MaxHeight;
            set => _MaxHeight = (uint)value;
        }
        public int MinHeight
        {
            get => (int)_MinHeight;
            set => _MinHeight = (uint)value;
        }
        public int MaxWidth
        {
            get => (int)_MaxWidth;
            set => _MaxWidth = (uint)value;
        }
        public int MinWidth
        {
            get => (int)_MinWidth;
            set => _MinWidth = (uint)value;
        }

        #endregion

        #region Functions

        public Size GetRandomSize()
        {
            var x1 = 0;
            var x2 = 0;
            var x3 = 0;
            var y1 = 0;
            var y2 = 0;
            var y3 = 0;

            if (MinWidth != -1)
                x1 = MinWidth;

            if (MaxWidth != -1)
                x2 = MaxWidth;
            else
                x2 = x1 + 1000;

            if (MinHeight != -1)
                y1 = MinHeight;

            if (MaxHeight != -1)
                y2 = MaxHeight;
            else
                y2 = y1 + 1000;

            x3 = new Random().Next(x1, x2 + 1);
            y3 = new Random().Next(y1, y2 + 1);

            return new Size(x3, y3);
        }

        public bool IsInRange(int width, int height = -1)
        {
            bool inWidth = false;
            bool inHeight = false;

            if (height != -1 && MaxHeight != -1)
                inHeight = (height >= MinHeight && height <= MaxHeight);
            else
                inHeight = true;

            inWidth = (width >= MinWidth);

            if (MaxWidth != -1)
                inWidth = (width <= MaxWidth);

            return inWidth && inHeight;
        }

        /// <summary>
        /// Compares the widths of the window sizes, ignores their heights
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is WindowSize)
            {
                var other = obj as WindowSize;

                if (MinWidth != -1 || other.MinWidth != -1)
                {
                    if (MinWidth > other.MinWidth)
                        return 1;
                    else if (MinWidth == other.MinWidth)
                        return 0;
                    else
                        return -1;
                }
                else if (MaxWidth != -1 || other.MaxWidth != -1)
                {
                    if (MaxWidth > other.MaxWidth)
                        return 1;
                    else if (MaxWidth == other.MaxWidth)
                        return 0;
                    else
                        return -1;
                }

                // Don't bother comparing heights, just return 0
                return 0;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static WindowSize[] OrderWindowSizesByWidth(WindowSize[] windowsizes)
        {
            var ordered = new List<WindowSize>();

            WindowSize smallestSize = new WindowSize(0);
            WindowSize largestSize = new WindowSize(0);

            foreach (var windowsize in windowsizes)
            {
                if (windowsize.CompareTo(smallestSize) == -1)
                {
                    smallestSize = windowsize;
                    ordered.Insert(0, windowsize);
                    continue;
                }
                else if (windowsize.CompareTo(largestSize) == 1)
                {
                    largestSize = windowsize;
                    ordered.Insert(ordered.Count, windowsize);
                    continue;
                }
                else
                {
                    for (int i = 0; i < ordered.Count; i++)
                    {
                        var compareResult = windowsize.CompareTo(windowsizes[i]);

                        if (compareResult == -1 || compareResult == 0)
                        {
                            ordered.Insert(i, windowsize);
                            break;
                        }
                    }

                    continue;
                }
            }

            return ordered.ToArray();
        }

        #endregion
    }
}
