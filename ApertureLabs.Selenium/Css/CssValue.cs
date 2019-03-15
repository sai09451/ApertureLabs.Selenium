using System;

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
        /// Initializes a new instance of the <see cref="CssValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException"></exception>
        public CssValue(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(value)} cannot be " +
                    $"null nor empty.");
            }

            Value = value;
            IsAuto = String.Equals(value, "auto", StringComparison.Ordinal);
            IsInherit = String.Equals(value, "inherit", StringComparison.Ordinal);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Original css string.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// True if the value is 'auto'.
        /// </summary>
        public bool IsAuto { get; private set; }

        /// <summary>
        /// True if the value is 'unset'.
        /// </summary>
        public bool IsUnset { get; private set; }

        /// <summary>
        /// True if the value is 'inherit'.
        /// </summary>
        public bool IsInherit { get; private set; }

        /// <summary>
        /// True if the the value is auto or inherit or unset.
        /// </summary>
        public bool IsCssWideKeyword => IsAuto || IsUnset || IsInherit;

        /// <summary>
        /// Returns the original css string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        #endregion

        #region Implicit Conversions

        /// <summary>
        /// Implicitly converts a CssValue to a string.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator string(CssValue value)
        {
            return value.ToString();
        }

        #endregion
    }
}
