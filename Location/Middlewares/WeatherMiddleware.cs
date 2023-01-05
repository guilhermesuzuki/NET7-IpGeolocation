using Location.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.IpLocation;
using Services.WeatherForecast;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Location.Middlewares
{
    /// <summary>
    /// Middleware class for setting the Location Details in Session
    /// </summary>
    public class WeatherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IWeatherService> _weatherServices;
        private readonly IUserFacade _userFacade;

        public WeatherMiddleware(
            RequestDelegate next,
            IEnumerable<IWeatherService> weatherServices,
            IUserFacade userFacade)
        {
            _next = next;
            _weatherServices = weatherServices;
            _userFacade = userFacade;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext)
        {
            //won't look for weather, if there's no geolocation
            if (_userFacade.Location == null)
            {
                await _next(httpContext);
                return;
            }

            //finds the weather
            //and saves into the facade
            _userFacade.Weather = _weatherServices.Next()?.Weather(
                new WeatherParameters
                {
                    City = this._userFacade.Location.City,
                    Country = this._userFacade.Location.Country,
                    CountryCode = this._userFacade.Location.CountryCode,
                    Latitude = this._userFacade.Location.Latitude,
                    Longitude = this._userFacade.Location.Longitude,
                    Region = this._userFacade.Location.Region,
                    RegionCode = this._userFacade.Location.RegionCode,
                });

            await _next(httpContext);
        }
    }
}
