using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Services.WeatherForecast.WeatherModel;

namespace Services.WeatherForecast
{
    public class ForecastModel : IWeather
    {
        /// <summary>
        /// simple function/delegate to convert celsius to fahrenheit
        /// </summary>
        public static int FromCelsiusToFahrenheit(int celsius)
        {
            return (int)(celsius * 1.8) + 32;
        }

        /// <summary>
        /// enumeration for weather conditions
        /// </summary>
        public enum eWeatherConditions : byte
        {
            /// <summary>
            /// Non Determined
            /// </summary>
            None = 0,

            /// <summary>
            /// day clear sky
            /// </summary>
            ClearSky,

            /// <summary>
            /// few clouds
            /// </summary>
            FewClouds,

            /// <summary>
            /// Scattered Clouds
            /// </summary>
            ScatteredClouds,

            /// <summary>
            /// Broken Clouds
            /// </summary>
            BrokenClouds,

            /// <summary>
            /// Shower Rain
            /// </summary>
            ShowerRain,

            /// <summary>
            /// Rain
            /// </summary>
            Rain,

            /// <summary>
            /// Thunderstorm
            /// </summary>
            Thunderstorm,

            /// <summary>
            /// Snow
            /// </summary>
            Snow,

            /// <summary>
            /// Mist or Fog
            /// </summary>
            Mist,

            /// <summary>
            /// Hail
            /// </summary>
            Hail,

            /// <summary>
            /// Windy
            /// </summary>
            Wind,

            /// <summary>
            /// Sleet
            /// </summary>
            Sleet,

            /// <summary>
            /// Tornado
            /// </summary>
            Tornado,
        }

        /// <summary>
        /// Date and Time of the Forecast
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// whether or not the sun has already set
        /// </summary>
        public bool IsNight { get; set; }

        /// <summary>
        /// Temperature in Celsius
        /// </summary>
        public int Temperature { get; set; }

        /// <summary>
        /// Minimum Temperature
        /// </summary>
        public int MinimumTemperature { get; set; }

        /// <summary>
        /// Maximum Temperature
        /// </summary>
        public int MaximumTemperature { get; set; }

        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public int TemperatureInFahrenheit
        {
            get { return FromCelsiusToFahrenheit(Temperature); }
        }

        /// <summary>
        /// Minimum Temperature in Fahrenheit
        /// </summary>
        public int MinimumTemperatureInFahrenheit
        {
            get { return FromCelsiusToFahrenheit(MinimumTemperature); }
        }

        /// <summary>
        /// Maximum Temperature in Fahrenheit
        /// </summary>
        public int MaximumTemperatureInFahrenheit
        {
            get { return FromCelsiusToFahrenheit(MaximumTemperature); }
        }

        /// <summary>
        /// Weather Condition
        /// </summary>
        public eWeatherConditions WeatherCondition { get; set; }

        /// <summary>
        /// Returns a string corresponding to the weather condition
        /// </summary>
        public virtual string WeatherDescription
        {
            get
            {
                try
                {
                    var description = Resources.Messages.ResourceManager.GetString(WeatherCondition.ToString());
                    if (string.IsNullOrWhiteSpace(description) == false) return description;
                }
                catch (Exception)
                {

                }

                return Resources.Messages.Unknown.ToString();
            }
        }

        /// <summary>
        /// Returns a css class
        /// </summary>
        public virtual string WeatherCode
        {
            get
            {
                return string.Concat(WeatherCondition, IsNight ? "-night" : string.Empty).ToLower();
            }
        }

        public string _Timestamp
        {
            get
            {
                return Timestamp.ToString("MMM dd HH:mm");
            }
        }

        public string _Timestamp_Date
        {
            get
            {
                return Timestamp.ToString("MMM dd");
            }
        }

        public string _Timestamp_Time
        {
            get
            {
                var language = Thread.CurrentThread.CurrentUICulture.Name.ToLower();

                var english = new string[] { "en-ca", "en-us", "en" };
                if (english.Contains(language) == true)
                {
                    return Timestamp.ToString("hhtt").TrimStart('0');
                }

                return Timestamp.ToString("HH") + "h";
            }
        }
    }
}
