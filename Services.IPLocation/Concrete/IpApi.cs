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
    /// <summary>
    /// IP-API.com doesn't require an API key to call their services, but there's a 45 requests/minute threshold.
    /// </summary>
    public class IpApi : LocationService
    {
        /// <summary>
        /// Sync Root object for multi-threading
        /// </summary>
        static object _SyncRoot = new object();

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="memoryCache"></param>
        public IpApi(IMemoryCache memoryCache)
            : base(memoryCache)
        {

        }

        /// <summary>
        /// 45 requests per minute
        /// </summary>
        public override int ThresoldLimit => 45;

        /// <summary>
        /// 45 requests per minute
        /// </summary>
        public override TimeSpan ThresholdExpiration => new TimeSpan(0, 1, 0);

        public override object SyncRoot => _SyncRoot;

        public override LocationModel Find(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip) == false)
            {
                try
                {
                    //composed url
                    var url = $"http://ip-api.com/json/{ip}";
                    var req = WebRequest.CreateHttp(url);
                    var res = req.GetResponse();

                    using (res)
                    {
                        var stream = res.GetResponseStream();
                        var reader = new StreamReader(stream);
                        var json = JObject.Parse(reader.ReadToEnd());

                        if (json.Value<string>("status") == "success" || json.Value<string>("status") != "fail")
                        {
                            //adds this call to the number of queries
                            this.NumberOfQueriesMade += 1;

                            return new LocationModel(ip)
                            {
                                City = json.Value<string>("city"),
                                Country = json.Value<string>("country"),
                                CountryCode = json.Value<string>("countryCode"),
                                Region = json.Value<string>("region"),
                                Latitude = string.IsNullOrWhiteSpace(json.Value<string>("lat")) == false ? json.Value<float>("lat") : (float?)null,
                                Longitude = string.IsNullOrWhiteSpace(json.Value<string>("lon")) == false ? json.Value<float>("lon") : (float?)null,
                                ZipCode = json.Value<string>("zip"),
                                TimeZone = json.Value<string>("timezone"),
                                //providers information
                                Provider = new string[] { "IP-API", "https://ip-api.com/" },
                            };
                        }
                        else
                        {
                            throw new Exception(json.Value<string>("message"));
                        }
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
