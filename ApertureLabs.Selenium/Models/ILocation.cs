namespace ApertureLabs.Selenium.Models
{
    /// <summary>
    /// Represents longitude/latitude.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Longitude
        /// </summary>
        decimal Longitude { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        decimal Latitude { get; set; }
    }
}
