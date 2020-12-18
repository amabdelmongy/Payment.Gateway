using System;
using Microsoft.AspNetCore.Mvc; 
using System.Collections.Generic;
using System.Linq;
using Service;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("weather-forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastDto> Get()
        {
            return _weatherForecastService.Get().Select(
                weatherForecast => new WeatherForecastDto
                {
                    Date = weatherForecast.Date,
                    Summary = weatherForecast.Summary,
                    Temperature = weatherForecast.Temperature
                }
            );
        }

        public class WeatherForecastDto
        {
            public DateTime Date { get; set; }

            public int Temperature { get; set; }

            public string Summary { get; set; }
        }
    }
}
