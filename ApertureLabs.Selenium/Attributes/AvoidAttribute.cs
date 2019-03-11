using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Attributes
{
    /// <summary>
    /// Avoid using any classes/methods/fields marked with this attribute in
    /// your code.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(validOn: AttributeTargets.All, AllowMultiple = false)]
    public class AvoidUsingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvoidUsingAttribute"/> class.
        /// </summary>
        public AvoidUsingAttribute()
        {
            Reason = "Avoid using the class/method/field this is applied to.";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AvoidUsingAttribute"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        public AvoidUsingAttribute(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }
}
