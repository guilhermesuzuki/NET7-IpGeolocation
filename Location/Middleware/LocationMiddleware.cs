using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Services.IpLocation;
using System.Net;
using System.Reflection.PortableExecutable;

namespace Location.Middleware
{
    /// <summary>
    /// Middleware class for setting the Location Details in Session
    /// </summary>
    public class LocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<ILocationService> _locationServices;

        public LocationMiddleware(RequestDelegate next, IEnumerable<ILocationService> locationServices)
        {
            this._next = next;
            this._locationServices = locationServices;
        }

        // IMessageWriter is injected into InvokeAsync
        public async Task InvokeAsync(HttpContext httpContext)
        {
            //if the ip-geolocation is already saved in session
            if (httpContext.Session.Get("ip-geolocation") != null)
            {
                await _next(httpContext);
                return;
            }

            //tries to find user's geolocation
            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            if (ip == "::1" || ip == "127.0.0.1")
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
            var iplocation = this._locationServices.Next().Find(ip);

            //and saves into session
            httpContext.Session.SetString("ip-geolocation", JsonConvert.SerializeObject(iplocation));

            await _next(httpContext);
        }
    }
}
