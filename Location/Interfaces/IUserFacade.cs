namespace Location.Interfaces
{
    /// <summary>
    /// Facade for User Information
    /// </summary>
    public interface IUserFacade
    {
        /// <summary>
        /// User Location
        /// </summary>
        public Services.IpLocation.LocationModel? Location { get; set; }

        /// <summary>
        /// User Weather
        /// </summary>
        public Services.WeatherForecast.WeatherModel? Weather { get; set; }
    }
}
