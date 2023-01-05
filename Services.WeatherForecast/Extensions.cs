using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.WeatherForecast
{
    /// <summary>
    /// Extensions for the Location Service
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Retrieves the next instance of IWeatherService available.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWeatherService? Next(this IEnumerable<IWeatherService> services)
        {
            if (services != null && services.Any() == true)
            {
                return services
                    .Where(x => x.IsUnderThresholdLimit)
                    .OrderBy(x => x.UnsuccessfulCalls)
                    .ThenBy(x => x.NumberOfQueriesMade)
                    .FirstOrDefault();
            }

            return null;
        }
    }
}
