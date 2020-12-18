using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Domain;
using Service;
using WebApi.Controllers;
using Moq;

namespace WebApi.Test
{
    public class WeatherForecastControllerTest
    {
        [Test]
        public void GIVEN_WeatherForecastController_WHEN_get_THEN_return_correct_List()
        {
            var expected = new WeatherForecast
            {
                Date = new DateTime(2020, 01, 01),
                Temperature = 1,
                Summary = "Warm"
            };

            var weatherForecastServiceMock = new Mock<IWeatherForecastService>();
            weatherForecastServiceMock
                .Setup(service => service.Get())
                .Returns(new List<WeatherForecast>
                    {
                        expected
                });

            var weatherForecastController = new WeatherForecastController(weatherForecastServiceMock.Object);
            var actualResult = weatherForecastController.Get();

            Assert.AreEqual(1, actualResult.Count());
            var actual = actualResult.First();
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Summary, actual.Summary);
            Assert.AreEqual(expected.Temperature, actual.Temperature);
        }
    }
}