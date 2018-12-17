using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Css
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICssGrammar
    {
        bool TryParse(string cssToken);
    }
}
