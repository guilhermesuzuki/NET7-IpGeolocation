using Microsoft.Extensions.Caching.Memory;
using Services.IpLocation;
using Services.WeatherForecast;

namespace Location.Facades
{
    public class UserFacade : Interfaces.IUserFacade
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IMemoryCache _memoryCache;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="memoryCache"></param>
        public UserFacade(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets/sets the user location in Cache.
        /// </summary>
        public LocationModel? Location
        {
            get
            {
                return this._memoryCache.Get<LocationModel>(
                    this._httpContextAccessor.HttpContext?.Session.Id + 
                    "-cache-iplocation");
            }
            set
            {
                this._memoryCache.Set(
                    this._httpContextAccessor.HttpContext?.Session.Id +
                    "-cache-iplocation",
                    value,
                    new MemoryCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 5, 0) });
            }
        }

        /// <summary>
        /// Gets/sets the user weather in Cache.
        /// </summary>
        public WeatherModel? Weather
        {
            get
            {
                return this._memoryCache.Get<WeatherModel>(
                    this._httpContextAccessor.HttpContext?.Session.Id +
                    "-cache-weather");
            }
            set
            {
                this._memoryCache.Set(
                    this._httpContextAccessor.HttpContext?.Session.Id +
                    "-cache-weather",
                    value,
                    new MemoryCacheEntryOptions { SlidingExpiration = new TimeSpan(0, 5, 0) });
            }
        }
    }
}
