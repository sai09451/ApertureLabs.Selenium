using System;
using System.Drawing;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Contains sizes for windows.
    /// </summary>
    public static class WindowSize
    {
        /// <summary>
        /// Default size for desktop (1001, 999).
        /// </summary>
        public static readonly Size DefaultDesktop = new Size(1201, 999);

        /// <summary>
        ///  Default size for tablet (768, 999).
        /// </summary>
        public static readonly Size DefaultTablet = new Size(768, 999);

        /// <summary>
        /// Default size for mobile (120, 999).
        /// </summary>
        public static readonly Size DefaultMobile = new Size(120, 999);

        /// <summary>
        /// Generates a random size in the given range. The height is always
        /// 999.
        /// </summary>
        /// <param name="minWidth"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        public static Size RandomSize(int minWidth, int maxWidth)
        {
            var rndWidth = new Random().Next(minWidth, maxWidth);
            return new Size(rndWidth, 999);
        }
    }
}
