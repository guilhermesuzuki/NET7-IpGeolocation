using Location.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.IpLocation;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Location.Middlewares
{
    /// <summary>
    /// Middleware class for setting the Location Details in Session
    /// </summary>
    public class LocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<ILocationService> _locationServices;
        private readonly IUserFacade _userFacade;

        public LocationMiddleware(
            RequestDelegate next, 
            IEnumerable<ILocationService> locationServices,
            IUserFacade userFacade)
        {
            _next = next;
            _locationServices = locationServices;
            _userFacade = userFacade;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext)
        {
            //if the ip-geolocation is already saved in session
            if (_userFacade.Location != null)
            {
                await _next(httpContext);
                return;
            }

            //tries to find user's real ip address
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ip) == true || ip == "::1" || ip == "127.0.0.1")
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //needs to get the public ip address from 'localhost'
                var url = "https://api.ipify.org/?format=json";
                var res = new HttpClient().GetStringAsync(url).Result;
                var json = JObject.Parse(res);

                ip = json.Value<string>("ip");
            }

            //finds the ip-geolocation
            //and saves into the facade
            _userFacade.Location = _locationServices.Next().Find(ip);

            await _next(httpContext);
        }
    }
}
