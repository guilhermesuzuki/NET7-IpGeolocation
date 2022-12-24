using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IpLocation.Concrete
{
    public class DummyIp : LocationService
    {
        public DummyIp(IMemoryCache memoryCache)
            : base(memoryCache)
        {

        }

        public override string KeyForCaching
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override TimeSpan ThresholdExpiration
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int ThresoldLimit
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override LocationModel Find(string ip)
        {
            throw new NotImplementedException();
        }
    }
}
