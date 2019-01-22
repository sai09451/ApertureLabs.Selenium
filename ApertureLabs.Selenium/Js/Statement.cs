using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Returns the content with an added semicolon if not present.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Content.Last() == ';')
                return Content;
            else
                return Content + ";";
        }

        #endregion
    }
}
