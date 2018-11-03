namespace ApertureLabs.Selenium.Models.NopModels
{
    /// <summary>
    /// Respsents an address according to NopCommerce.
    /// </summary>
    public interface INopAddress
    {
        /// <summary>
        /// FirstName.
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// LastName.
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Email.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Company.
        /// </summary>
        string Company { get; set; }

        /// <summary>
        /// Country.
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// StateProvice.
        /// </summary>
        string StateProvince { get; set; }

        /// <summary>
        /// City.
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Address1.
        /// </summary>
        string Address1 { get; set; }

        /// <summary>
        /// Address2.
        /// </summary>
        string Address2 { get; set; }

        /// <summary>
        /// PostalCode.
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// PhoneNumber
        /// </summary>
        string PhoneNumber { get; set; }

        /// <summary>
        /// FaxNumber.
        /// </summary>
        string FaxNumber { get; set; }
    }
}
