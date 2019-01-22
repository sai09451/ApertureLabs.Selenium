using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Js
{
    public class Statement : JavaScript
    {
        #region Constructor

        public Statement()
        {
            Content = String.Empty;
        }

        public Statement(string content)
        {
            Content = content;
        }

        #endregion

        #region Properties

        public string Content { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}
