using ApertureLabs.Selenium.Extensions;
using System;

namespace ApertureLabs.Selenium
{
    /// <summary>
    /// Used for generating date-of-births, names, etc...
    /// </summary>
    public class Data
    {
        #region Fields

        private readonly string apiKey;

        #endregion

        #region Constructor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="apiKey">
        /// The google maps api key.
        /// </param>
        public Data(string apiKey)
        {
            this.apiKey = apiKey;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Selects a random city in a country and state/province.
        /// </summary>
        /// <param name="country">
        /// Can be US or CN for United States and Canada respectively.
        /// </param>
        /// <param name="stateOrProvince">
        /// Should be the two digit state/province code.
        /// </param>
        /// <returns></returns>
        public string RandomCity(string country, string stateOrProvince)
        {
            string[] cities;

            switch (country)
            {
                case "US":
                    {
                        switch (stateOrProvince)
                        {
                            case "AL":
                                cities = new string[]
                                {
                                    "Montogomery"
                                };

                                break;

                            case "AK":
                                cities = new string[]
                                {
                                    "Juneau"
                                };

                                break;

                            case "AZ":
                                cities = new string[]
                                {
                                    "Phoenix"
                                };

                                break;

                            case "AR":
                                cities = new string[]
                                {
                                    "Little Rock"
                                };

                                break;

                            case "CA":
                                cities = new string[]
                                {
                                    "Sacramento"
                                };

                                break;

                            case "CO":
                                cities = new string[]
                                {
                                    "Denver"
                                };

                                break;

                            case "CT":
                                cities = new string[]
                                {
                                    "Hartford"
                                };

                                break;

                            case "DE":
                                cities = new string[]
                                {
                                    "Dover"
                                };

                                break;

                            case "FL":
                                cities = new string[]
                                {
                                    "Tallahassee"
                                };

                                break;

                            case "GA":
                                cities = new string[]
                                {
                                    "Atlanta"
                                };

                                break;

                            case "HI":
                                cities = new string[]
                                {
                                    "Honolulu"
                                };

                                break;

                            case "ID":
                                cities = new string[]
                                {
                                    "Boise"
                                };

                                break;

                            case "IL":
                                cities = new string[]
                                {
                                    "Springfield"
                                };

                                break;

                            case "IN":
                                cities = new string[]
                                {
                                    "Indianapolis"
                                };

                                break;

                            case "IA":
                                cities = new string[]
                                {
                                    "Des Moines"
                                };

                                break;

                            case "KS":
                                cities = new string[]
                                {
                                    "Topeka"
                                };

                                break;

                            case "KY":
                                cities = new string[]
                                {
                                    "Frankfort"
                                };

                                break;

                            case "LA":
                                cities = new string[]
                                {
                                    "Baton Rouge"
                                };

                                break;

                            case "ME":
                                cities = new string[]
                                {
                                    "Augusta"
                                };

                                break;

                            case "MA":
                                cities = new string[]
                                {
                                    "Annapolis"
                                };

                                break;

                            case "MI":
                                cities = new string[]
                                {
                                    "Lansing"
                                };

                                break;

                            case "MN":
                                cities = new string[]
                                {
                                    "Saint Paul"
                                };

                                break;

                            case "MS":
                                cities = new string[]
                                {
                                    "Jackson"
                                };

                                break;

                            case "MO":
                                cities = new string[]
                                {
                                    "Jefferson City"
                                };

                                break;

                            case "MT":
                                cities = new string[]
                                {
                                    "Helena"
                                };

                                break;

                            case "NE":
                                cities = new string[]
                                {
                                    "Lincoln"
                                };

                                break;

                            case "NV":
                                cities = new string[]
                                {
                                    "Carson City"
                                };

                                break;

                            case "NH":
                                cities = new string[]
                                {
                                    "Concord"
                                };

                                break;

                            case "NJ":
                                cities = new string[]
                                {
                                    "Trenton"
                                };

                                break;

                            case "NM":
                                cities = new string[]
                                {
                                    "Santa Fe"
                                };

                                break;

                            case "NY":
                                cities = new string[]
                                {
                                    "Albany"
                                };

                                break;

                            case "NC":
                                cities = new string[]
                                {
                                    "Raleigh"
                                };

                                break;

                            case "ND":
                                cities = new string[]
                                {
                                    "Bismarck"
                                };

                                break;

                            case "OH":
                                cities = new string[]
                                {
                                    "Columbus"
                                };

                                break;

                            case "OK":
                                cities = new string[]
                                {
                                    "Oklahoma City"
                                };

                                break;

                            case "OR":
                                cities = new string[]
                                {
                                    "Salem"
                                };

                                break;

                            case "PA":
                                cities = new string[]
                                {
                                    "Harrisburg"
                                };

                                break;

                            case "RI":
                                cities = new string[]
                                {
                                    "Providence"
                                };

                                break;

                            case "SC":
                                cities = new string[]
                                {
                                    "Columbia"
                                };

                                break;

                            case "SD":
                                cities = new string[]
                                {
                                    "Pierre"
                                };

                                break;

                            case "TN":
                                cities = new string[]
                                {
                                    "Nashville"
                                };

                                break;

                            case "TX":
                                cities = new string[]
                                {
                                    "Austin"
                                };

                                break;

                            case "UT":
                                cities = new string[]
                                {
                                    "Salt Lake City"
                                };

                                break;

                            case "VT":
                                cities = new string[]
                                {
                                    "Montpelier"
                                };

                                break;

                            case "VA":
                                cities = new string[]
                                {
                                    "Richmond"
                                };

                                break;

                            case "WA":
                                cities = new string[]
                                {
                                    "Olympia"
                                };

                                break;

                            case "WV":
                                cities = new string[]
                                {
                                    "Charleston"
                                };

                                break;


                            case "WI":
                                cities = new string[]
                                {
                                    "Waukesha",
                                    "Madison"
                                };

                                break;

                            case "WY":
                                cities = new string[]
                                {
                                    "Cheyenne"
                                };

                                break;


                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    }

                case "CN":
                    {
                        switch (stateOrProvince)
                        {
                            case "ON":
                                cities = new string[]
                                {
                                    "Toronto"
                                };

                                break;

                            case "QC":
                                cities = new string[]
                                {
                                    "Quebec City",
                                    "Motreal"
                                };

                                break;

                            case "NS":
                                cities = new string[]
                                {
                                    "Halifax"
                                };

                                break;

                            case "NB":
                                cities = new string[]
                                {
                                    "Fredericton",
                                    "Moncton"
                                };

                                break;

                            case "MB":
                                cities = new string[]
                                {
                                    "Winnipeg"
                                };

                                break;

                            case "BC":
                                cities = new string[]
                                {
                                    "Victoria",
                                    "Vancouver"
                                };

                                break;

                            case "PE":
                                cities = new string[]
                                {
                                    "Charlottetown"
                                };

                                break;

                            case "SK":
                                cities = new string[]
                                {
                                    "Regina",
                                    "Saskatoon"
                                };

                                break;

                            case "AB":
                                cities = new string[]
                                {
                                    "Endmonton",
                                    "Calgary"
                                };

                                break;

                            case "NL":
                                cities = new string[]
                                {
                                    "St. John's"
                                };

                                break;

                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }

            return cities.SelectRandom();
        }

        /// <summary>
        /// Selects a random country. Currently only returns 'Canada',
        /// 'United Kingdom', and the 'United States'.
        /// </summary>
        /// <returns>Full legal name of the country.</returns>
        public string RandomCountry()
        {
            var countries = new string[]
            {
                "Canada",
                "United Kingdom",
                "United States",
            };

            return countries.SelectRandom();
        }

        /// <summary>
        /// Returns a random state/province of a country.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public string RandomStateOrProvince(string country)
        {
            string[] statesAndProvinces;

            switch (country)
            {
                case "US":
                case "United States":
                    statesAndProvinces = new string[]
                    {
                        "Alabama",
                        "Alaska",
                        "Arizona",
                        "Arkansas",
                        "California",
                        "Colorado",
                        "Connecticut",
                        "Delaware",
                        "Florida",
                        "Georgia",
                        "Hawaii",
                        "Idaho",
                        "Illinois",
                        "Indiana",
                        "Iowa",
                        "Kansas",
                        "Kentucky",
                        "Louisiana",
                        "Maine",
                        "Maryland",
                        "Massachusetts",
                        "Michigan",
                        "Minnesota",
                        "Mississippi",
                        "Missouri",
                        "Montana",
                        "Nebraska",
                        "Nevada",
                        "New Hampshire",
                        "New Jersey",
                        "New Mexico",
                        "New York",
                        "North Carolina",
                        "North Dakota",
                        "Ohio",
                        "Oklahoma",
                        "Oregon",
                        "Pennsylvania",
                        "Rhode Island",
                        "South Carolina",
                        "South Dakota",
                        "Tennessee",
                        "Texas",
                        "Utah",
                        "Vermont",
                        "Virginia",
                        "Washington",
                        "West Virginia",
                        "Wisconsin",
                        "Wyoming"
                    };

                    break;

                default:
                    throw new NotImplementedException();
            }

            int rnd = new Random().Next(0, statesAndProvinces.Length);

            return statesAndProvinces[rnd];
        }

        /// <summary>
        /// The returned date is guaranteed to be at least 18 years in the past if
        /// the argument adult is true, if false the returned date will NOT be 18
        /// years in the past
        /// </summary>
        /// <returns></returns>
        public DateTime RandomDateOfBirth(bool adult = true)
        {
            var random = new Random();
            int rndYear;
            int rndDays = random.Next(0, 365);

            rndYear = adult ? random.Next(18, 75) : random.Next(5, 18);

            var bday = DateTime.Today;

            var timeSpan = TimeSpan.FromDays((rndYear * 365) + rndDays);

            bday = bday.Subtract(timeSpan);

            return bday;
        }

        /// <summary>
        /// Generates a string of random letters (uses both upper case and
        /// lower case).
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string RandomString(int length)
        {
            var chars = new char[length];
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var @char = random.Next(65, 91);
                var @case = random.Next(0, 2) == 0;

                if (@case)
                    @char += 32;

                chars[i] += (char)@char;
            }

            return String.Concat(chars);
        }

        #endregion
    }
}
