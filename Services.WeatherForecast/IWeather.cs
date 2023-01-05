using System;

namespace Services.WeatherForecast
{
    public interface IWeather
    {
        string WeatherCode { get; }
        int MaximumTemperature { get; set; }
        int MaximumTemperatureInFahrenheit { get; }
        int MinimumTemperature { get; set; }
        int MinimumTemperatureInFahrenheit { get; }
        int Temperature { get; set; }
        int TemperatureInFahrenheit { get; }
        ForecastModel.eWeatherConditions WeatherCondition { get; set; }
        string WeatherDescription { get; }
        bool IsNight { get; }
    }
}