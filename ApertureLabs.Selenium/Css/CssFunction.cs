using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ApertureLabs.Selenium.Css
{
    class CssFunction : CssValue
    {
        #region Fields

        private readonly Regex argumentRegex;

        #endregion

        #region Constructor

        public CssFunction(string value) : base(value)
        { }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public CssValue GetArguments()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
