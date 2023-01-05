using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.WeatherInformation.Concrete
{
    public class DummyWeather : WeatherService
    {
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

        public override WeatherModel Weather(IWeatherParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
