using System;
using System.Collections.Generic;
using Domain;

namespace Service
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }

    public class WeatherForecastService : IWeatherForecastService
    {
        public IEnumerable<WeatherForecast> Get()
        {
            return new List<WeatherForecast>
            { new WeatherForecast {
                    Date = new DateTime(2020,01,01),
                    Temperature = 1,
                    Summary = "Warm"
                }
            };
        }
    }
}
