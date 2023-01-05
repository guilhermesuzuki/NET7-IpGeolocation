using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.WeatherForecast
{
    /// <summary>
    /// Weather uses Metric system (Celsius unity). You can use methods to convert the temperatures to F.
    /// </summary>
    public class WeatherModel : ForecastModel
    {
        /// <summary>
        /// default constructor
        /// </summary>
        public WeatherModel() : base()
        {
            Forecast = new List<ForecastModel>();
        }

        /// <summary>
        /// Location was Provided By
        /// </summary>
        public string[] LocationBy { get; set; } = new string[2] { string.Empty, string.Empty };

        /// <summary>
        /// longitude of the location
        /// </summary>
        public float? Longitude { get; set; }

        /// <summary>
        /// latitude of the location
        /// </summary>
        public float? Latitude { get; set; }

        /// <summary>
        /// name of the city
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State, province or whatever
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Region code
        /// </summary>
        public string RegionCode { get; set; }

        /// <summary>
        /// name of the country
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Country code
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// ZipCode when present
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// Timezone
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Forecast
        /// </summary>
        public List<ForecastModel> Forecast { get; set; }

        /// <summary>
        /// Humidity percentage
        /// </summary>
        public byte? Humidity { get; set; }

        /// <summary>
        /// Indicates whether there's location or not
        /// </summary>
        public bool HasLocation
        {
            get
            {
                if (Latitude.HasValue && Longitude.HasValue) return true;
                if (string.IsNullOrWhiteSpace(Country) == false && string.IsNullOrWhiteSpace(City) == false) return true;

                return false;
            }
        }

        /// <summary>
        /// Indicates whether there's weather or not
        /// </summary>
        public bool HasWeather
        {
            get
            {
                //if (this.Exception != null) return false;
                if (HasLocation) return WeatherCondition != eWeatherConditions.None;
                return false;
            }
        }

        /// <summary>
        /// Location was Provided By
        /// </summary>
        public string[] WeatherProvider { get; set; } = new string[2] { string.Empty, string.Empty };

        /// <summary>
        /// 
        /// </summary>
        public string[] WeatherBy
        {
            get
            {
                if (WeatherProvider != null && WeatherProvider.Length > 1)
                {
                    return new string[2]
                    {
                        string.Format(Resources.Messages.ProvidedBy.ToLower(), WeatherProvider[0]),
                        WeatherProvider[1],
                    };
                }

                return new string[2] { string.Empty, string.Empty };
            }
        }

        /// <summary>
        /// Weather Description
        /// </summary>
        public override string WeatherDescription
        {
            get
            {
                if (HasWeather) return base.WeatherDescription;
                return MessageForServiceUnavailable;
            }
        }

        /// <summary>
        /// Service unavailable message
        /// </summary>
        public string MessageForServiceUnavailable
        {
            get
            {
                return Resources.Messages.ServiceUnavailable;
            }
        }
    }
}
