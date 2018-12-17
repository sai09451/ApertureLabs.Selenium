using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// Representing and converting css values (length, color, angle, etc...).
    /// </summary>
    public class CssValue
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="value"></param>
        public CssValue(string value)
        {
            Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original css string.
        /// </summary>
        public string Value { get; private set; }

        #endregion
    }
}
