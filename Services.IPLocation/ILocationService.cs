using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IpLocation
{
    /// <summary>
    /// Location service, but those that work with threshold and number of requests by client/app key
    /// </summary>
    public interface ILocationService
    {
        /// <summary>
        /// Api Key (service provider requires one, so register it first)
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Number of unsuccessful calls
        /// </summary>
        int UnsuccessfulCalls { get; }

        /// <summary>
        /// How many calls were made to it
        /// </summary>
        int NumberOfQueriesMade { get; }

        /// <summary>
        /// Initial limit for the threshold
        /// </summary>
        int ThresoldLimit { get; }

        /// <summary>
        /// must find the location based on the ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        LocationModel Find(string ip);

        /// <summary>
        /// Indicates whether this service can be called, meaning that the caller can use this instance because the number of callings is under the threshold limit.
        /// </summary>
        bool IsUnderThresholdLimit { get; }

        /// <summary>
        /// Key for Caching
        /// </summary>
        string KeyForCaching { get; }

        /// <summary>
        /// Key for Caching Unsuccessful Calls
        /// </summary>
        string KeyForCachingUnsuccessfulCalls { get; }

        /// <summary>
        /// Key for Caching Number of Queries Made
        /// </summary>
        string KeyForCachingNumberOfQueriesMade { get; }

        /// <summary>
        /// Key for Caching the Thresold Limit
        /// </summary>
        string KeyForCachingThresoldLimit { get; }
    }
}
