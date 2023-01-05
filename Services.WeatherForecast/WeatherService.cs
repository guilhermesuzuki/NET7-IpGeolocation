using Microsoft.Extensions.Caching.Memory;

namespace Services.WeatherForecast
{
    public abstract class WeatherService : IWeatherService
    {
        /// <summary>
        /// Cache instance
        /// </summary>
        protected IMemoryCache _memoryCache;

        /// <summary>
        /// Constructor with DI
        /// </summary>
        /// <param name="memoryCache"></param>
        public WeatherService(IMemoryCache memoryCache)
            : base()
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Whether or not this instance of the service has maxed up
        /// </summary>
        public virtual bool IsUnderThresholdLimit
        {
            get { return NumberOfQueriesMade + UnsuccessfulCalls < ThresoldLimit; }
        }

        /// <summary>
        /// key string for caching
        /// </summary>
        public abstract string KeyForCaching { get; }

        /// <summary>
        /// Sync object for racing conditions
        /// </summary>
        public abstract object SyncRoot { get; }

        /// <summary>
        /// Number of Unsuccessful calls made
        /// </summary>
        public int UnsuccessfulCalls
        {
            get
            {
                var key = $"{KeyForCaching}/UnsuccessfulCalls";
                return _memoryCache.Get(key) == null ? 0 : _memoryCache.Get<int>(key);
            }
            set
            {
                _memoryCache.Set($"{KeyForCaching}/UnsuccessfulCalls", value);
            }
        }

        /// <summary>
        /// Number of queries made so far with this instance
        /// </summary>
        public virtual int NumberOfQueriesMade
        {
            get
            {
                var key = $"{KeyForCaching}/NumberOfQueriesMade";

                //first, it initializes the cache, if not present
                if (_memoryCache.Get(key) == null)
                {
                    lock (SyncRoot)
                    {
                        if (_memoryCache.Get(key) == null)
                        {
                            NumberOfQueriesMade = 0;
                            return 0;
                        }
                    }
                }

                return _memoryCache.Get<int>(key);
            }

            set
            {
                var key = $"{KeyForCaching}/NumberOfQueriesMade";

                //first it removes the item from cache
                if (_memoryCache.Get(key) != null)
                {
                    lock (SyncRoot)
                    {
                        if (_memoryCache.Get(key) != null)
                        {
                            _memoryCache.Remove(key);
                        }
                    }
                }

                //memory cache entry options with absolute expiration
                var memoptions = new MemoryCacheEntryOptions { AbsoluteExpiration = ThresholdExpiration, };

                //adds a new eviction callback to remove some other stuff from cache as well.
                memoptions.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
                {
                    EvictionCallback = (evictionKey, evictionValue, evictionReason, evctionState) =>
                    {
                        _memoryCache.Remove($"{KeyForCaching}/UnsuccessfulCalls");
                    }
                });

                //adds the value in cache with the memory cache options.
                _memoryCache.Set(key, value, memoptions);
            }
        }

        /// <summary>
        /// Default false. It means that a call needs to be made in order to obtain forecast.
        /// </summary>
        public virtual bool ProvidesForecastWithoutAnotherCall
        {
            get { return false; }
        }

        public abstract int ThresoldLimit { get; }

        /// <summary>
        /// In case this service provides forecasts within the call of the Weather service
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual List<ForecastModel> Forecast(IWeatherParameters parameters)
        {
            if (ProvidesForecastWithoutAnotherCall)
            {
                var weatherInfo = Weather(parameters);
                return weatherInfo.Forecast;
            }

            throw new NotImplementedException();
        }

        public abstract WeatherModel Weather(IWeatherParameters parameters);

        /// <summary>
        /// Date and Time when the threshold limit will be set to zero, enabling the instance to be called again
        /// </summary>
        public abstract DateTime ThresholdExpiration { get; }
    }
}
