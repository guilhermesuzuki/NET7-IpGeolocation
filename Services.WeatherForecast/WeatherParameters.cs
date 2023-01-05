using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.WeatherForecast
{
    public class WeatherParameters : IWeatherParameters
    {
        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string CountryCode { get; set; } = string.Empty;

        public float? Latitude
        {
            get; set;
        }

        public float? Longitude
        {
            get; set;
        }

        public string Region { get; set; } = string.Empty;

        public string RegionCode { get; set; } = string.Empty;
    }
}
