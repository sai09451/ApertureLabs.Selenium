using System;
using System.Collections.Generic;
using System.Text;

namespace ApertureLabs.Selenium.Models
{
    /// <summary>
    /// IAddress.
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// The lat/lng of the address.
        /// </summary>
        ILocation Location { get; set; }
    }
}
