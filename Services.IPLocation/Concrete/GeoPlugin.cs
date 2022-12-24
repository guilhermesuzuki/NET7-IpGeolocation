using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.IpLocation.Concrete
{
    public class GeoPlugin : LocationService
    {
        /// <summary>
        /// Sync Root object for multi-threading
        /// </summary>
        static object _SyncRoot = new object();

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="memoryCache"></param>
        public GeoPlugin(IMemoryCache memoryCache)
            : base(memoryCache)
        {

        }

        /// <summary>
        /// 120 lookups per minute
        /// </summary>
        public override int ThresoldLimit => 120;

        /// <summary>
        /// 120 lookups per minute
        /// </summary>
        public override TimeSpan ThresholdExpiration => new TimeSpan(0, 1, 0);

        /// <summary>
        /// SyncRoot object
        /// </summary>
        public override object SyncRoot => _SyncRoot;

        public override LocationModel Find(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) == false)
            {
                try
                {
                    //composed url
                    var url = $"http://www.geoplugin.net/json.gp?ip={ip}";
                    var res = new HttpClient().GetStringAsync(url).Result;
                    var json = JObject.Parse(res);

                    if (json.Value<int>("geoplugin_status") == 200)
                    {
                        //adds this call to the number of queries
                        this.NumberOfQueriesMade += 1;

                        return new LocationModel(ip)
                        {
                            City = json.Value<string>("geoplugin_city"),
                            Country = json.Value<string>("geoplugin_countryName"),
                            CountryCode = json.Value<string>("geoplugin_countryCode"),
                            Region = json.Value<string>("geoplugin_region"),
                            Latitude = string.IsNullOrWhiteSpace(json.Value<string>("geoplugin_latitude")) == false ? json.Value<float>("geoplugin_latitude") : (float?)null,
                            Longitude = string.IsNullOrWhiteSpace(json.Value<string>("geoplugin_longitude")) == false ? json.Value<float>("geoplugin_longitude") : (float?)null,
                            //ZipCode = json.Value<string>("zip"),
                            TimeZone = json.Value<string>("geoplugin_timezone"),
                            //providers information
                            Provider = new string[] { "GeoPlugin", "https://www.geoplugin.com/" },
                        };
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    this.UnsuccessfulCalls += 1;
                    throw;
                }
            }

            //so it can be catch with the proper message from the framework
            throw new ArgumentNullException("ip");
        }
    }
}
