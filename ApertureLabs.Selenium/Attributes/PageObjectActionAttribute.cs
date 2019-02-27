using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
    public class PageObjectActionAttribute : Attribute
    {
        public PageObjectActionAttribute()
        { }
    }
}
