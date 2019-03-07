using System;
using System.Collections.Generic;
using System.Globalization;

namespace ApertureLabs.Selenium.Models.NopModels
{
    /// <summary>
    /// Default implementation of INopAddress.
    /// </summary>
    public class NopAddress : INopAddress
    {
        #region Fields

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public NopAddress()
        { }

        /// <summary>
        /// Parses a string into a NopAddress.
        /// </summary>
        /// <param name="address"></param>
        public NopAddress(string address)
        {
            if (String.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            var parts = address.Split(',');

            // Verify format of address.
            if (parts.Length != 5)
            {
                throw new ArgumentException($"{nameof(address)} has invalid" +
                    $"format. Must match: 'Firstname Lastname, Address1, City," +
                    $"StateProvince PostalCode, Country");
            }

            // Name.
            var nameParts = parts[0].Split(' ');
            FirstName = nameParts[0];
            LastName = nameParts[1];

            // Address.
            Address1 = parts[1];

            // StateProvince and postal code.
            var stateAndPostalParts = parts[2].Split(' ');
            StateProvince = stateAndPostalParts[0];
            PostalCode = stateAndPostalParts[1];

            // Country.
            Country = parts[4];
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string FirstName { get; set; }

        /// <inheritdoc/>
        public string LastName { get; set; }

        /// <inheritdoc/>
        public string Email { get; set; }

        /// <inheritdoc/>
        public string Company { get; set; }

        /// <inheritdoc/>
        public string Country { get; set; }

        /// <inheritdoc/>
        public string StateProvince { get; set; }

        /// <inheritdoc/>
        public string City { get; set; }

        /// <inheritdoc/>
        public string Address1 { get; set; }

        /// <inheritdoc/>
        public string Address2 { get; set; }

        /// <inheritdoc/>
        public string PostalCode { get; set; }

        /// <inheritdoc/>
        public string PhoneNumber { get; set; }

        /// <inheritdoc/>
        public string FaxNumber { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is INopAddress address
                ? Equals(address)
                : base.Equals(obj);
        }

        #endregion

        #region Methods

#pragma warning disable CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning disable CS1658 // Warning is overriding an error

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <seealso cref="https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416"/>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap.
            {
                int hash = 17;
                hash = hash * 23 + FirstName?.GetHashCode() ?? 0;
                hash = hash * 23 + LastName?.GetHashCode() ?? 0;
                hash = hash * 23 + Email?.GetHashCode() ?? 0;
                hash = hash * 23 + Country?.GetHashCode() ?? 0;
                hash = hash * 23 + StateProvince?.GetHashCode() ?? 0;
                hash = hash * 23 + City?.GetHashCode() ?? 0;
                hash = hash * 23 + Address1?.GetHashCode() ?? 0;
                hash = hash * 23 + Address2?.GetHashCode() ?? 0;
                hash = hash * 23 + PostalCode?.GetHashCode() ?? 0;
                hash = hash * 23 + PhoneNumber?.GetHashCode() ?? 0;
                return hash;
            }
        }

#pragma warning restore CS1584 // XML comment has syntactically incorrect cref attribute
#pragma warning restore CS1658 // Warning is overriding an error

        /// <summary>
        /// Checks if the addresses are the same.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool Equals(INopAddress address)
        {
            // Local function for readability.
            bool isEqual(IDictionary<string, string> compare)
            {
                var areEqual = true;

                foreach (var key in compare.Keys)
                {
                    var value = compare[key];

                    if (String.Equals(key, value, StringComparison.Ordinal))
                    {
                        areEqual = false;
                        break;
                    }
                }

                return areEqual;
            }

            return isEqual(new Dictionary<string, string>()
            {
                { Address1, address.Address1 },
                { Address2, address.Address2 },
                { City, address.City },
                { Company, address.Company },
                { Country, address.Country },
                { Email, address.Email },
                { FaxNumber, address.FaxNumber },
                { FirstName, address.FirstName },
                { LastName, address.LastName },
                { PhoneNumber, address.PhoneNumber },
                { PostalCode, address.PostalCode },
                { StateProvince, address.StateProvince }
            });
        }

        /// <summary>
        /// Returns an address string that is usually formatted identical to
        /// ones displayed by Nop.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture,
                "{0} {1}, {2}, {3}, {4} {5}, {6}",
                FirstName,
                LastName,
                Address1,
                City,
                StateProvince,
                PostalCode,
                Country);
        }

        /// <summary>
        /// Checks if this is a valid address.
        /// </summary>
        /// <returns></returns>
        public bool IsValidAddress()
        {
            return !String.IsNullOrWhiteSpace(Address1)
                && !String.IsNullOrWhiteSpace(City)
                && !String.IsNullOrWhiteSpace(Country)
                && !String.IsNullOrWhiteSpace(Email)
                && !String.IsNullOrWhiteSpace(FirstName)
                && !String.IsNullOrWhiteSpace(Address1)
                && !String.IsNullOrWhiteSpace(LastName)
                && !String.IsNullOrWhiteSpace(PhoneNumber)
                && !String.IsNullOrWhiteSpace(PostalCode)
                && !String.IsNullOrWhiteSpace(StateProvince);
        }

        #endregion
    }
}
