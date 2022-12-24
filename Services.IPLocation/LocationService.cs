using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Services.IpLocation
{
    /// <summary>
    /// abstract class for a location service
    /// </summary>
    public abstract class LocationService : ILocationService
    {
        /// <summary>
        /// GUID as a replacement for the Api Key
        /// </summary>
        protected readonly Guid _apiKey = new Guid();

        /// <summary>
        /// Memory Cache
        /// </summary>
        protected readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Constructor with DI.
        /// </summary>
        /// <param name="memoryCache"></param>
        public LocationService(IMemoryCache memoryCache)
            : base()
        {
            this._memoryCache = memoryCache;
        }

        public virtual string ApiKey
        {
            get { return this._apiKey.ToString().ToLower(); }
            protected set { }
        }

        /// <summary>
        /// key string for caching
        /// </summary>
        public virtual string KeyForCaching
        {
            get { return $"{this.GetType().Name.ToLower()}-{this.ApiKey}"; }
        }

        public string KeyForCachingUnsuccessfulCalls => $"{this.KeyForCaching}/UnsuccessfulCalls";
        public string KeyForCachingNumberOfQueriesMade => $"{this.KeyForCaching}/NumberOfQueriesMade";
        public string KeyForCachingThresoldLimit => $"{this.KeyForCaching}/ThresoldLimit";

        /// <summary>
        /// Key for caching the DateAndTimeTheCacheExpires
        /// </summary>
        public string KeyForCachingDateAndTimeTheCacheExpires => $"{this.KeyForCaching}/DateAndTimeTheCacheExpires";

        /// <summary>
        /// Whether or not this instance of the service has maxed up
        /// </summary>
        public virtual bool IsUnderThresholdLimit
        {
            get
            {
                return (this.NumberOfQueriesMade + this.UnsuccessfulCalls) < this.ThresoldLimit;
            }
        }

        /// <summary>
        /// Number of Unsuccessful calls made
        /// </summary>
        public int UnsuccessfulCalls
        {
            get
            {
                var key = this.KeyForCachingUnsuccessfulCalls;
                return this._memoryCache.Get(key) == null ? 0 : this._memoryCache.Get<int>(key);
            }
            set
            {
                this._memoryCache.Set<int>(this.KeyForCachingUnsuccessfulCalls, value);
            }
        }

        /// <summary>
        /// Number of queries made so far with this instance
        /// </summary>
        public virtual int NumberOfQueriesMade
        {
            get
            {
                var key = this.KeyForCachingNumberOfQueriesMade;

                if (this._memoryCache.Get(key) == null)
                {
                    lock (SyncRoot)
                    {
                        if (this._memoryCache.Get(key) == null)
                        {
                            //cache options
                            var options = new MemoryCacheEntryOptions
                            {
                                AbsoluteExpiration = this.DateAndTimeTheCacheExpires,
                            };

                            //register the post eviction callback
                            options.RegisterPostEvictionCallback((k, v, reason, state) =>
                            {
                                this._memoryCache.Remove(this.KeyForCachingUnsuccessfulCalls);
                            });

                            //adds the value in cache
                            this._memoryCache.Set(key, 0, options);
                        }
                    }
                }

                return this._memoryCache.Get(key) != null ? this._memoryCache.Get<int>(key) : 0;
            }

            set
            {
                var key = this.KeyForCachingNumberOfQueriesMade;

                //cache options
                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = this.DateAndTimeTheCacheExpires,
                };

                //register the post eviction callback
                options.RegisterPostEvictionCallback((k, v, reason, state) =>
                {
                    this._memoryCache.Remove(this.KeyForCachingUnsuccessfulCalls);
                });

                //adds the value in cache
                this._memoryCache.Set(key, value, options);
            }
        }

        /// <summary>
        /// The threshold limit for each instance of the service
        /// </summary>
        public abstract int ThresoldLimit
        {
            get;
        }

        /// <summary>
        /// Finds a location based on its ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public abstract LocationModel Find(string ip);

        /// <summary>
        /// Timespan the threshold limit will be set to zero, enabling the instance to be called again
        /// </summary>
        public abstract TimeSpan ThresholdExpiration { get; }

        /// <summary>
        /// Date and Time the Cache Expires
        /// </summary>
        protected virtual DateTime DateAndTimeTheCacheExpires
        {
            get
            {
                var dtm = DateTime.Now.Add(this.ThresholdExpiration);
                var key = this.KeyForCachingDateAndTimeTheCacheExpires;

                if (this._memoryCache.Get(key) == null)
                {
                    lock (SyncRoot)
                    {
                        if (this._memoryCache.Get(key) == null)
                        {
                            var options = new MemoryCacheEntryOptions { AbsoluteExpiration = dtm, };

                            //register the post eviction callback
                            options.RegisterPostEvictionCallback((k, v, reason, state) =>
                            {
                                this._memoryCache.Remove(this.KeyForCachingNumberOfQueriesMade);
                                this._memoryCache.Remove(this.KeyForCachingUnsuccessfulCalls);
                            });

                            this._memoryCache.Set(key, dtm, options);
                        }
                    }
                }

                return this._memoryCache.Get(key) != null ? this._memoryCache.Get<DateTime>(key) : dtm;
            }
        }

        /// <summary>
        /// Sync object for racing conditions
        /// </summary>
        public abstract object SyncRoot { get; }
    }
}
