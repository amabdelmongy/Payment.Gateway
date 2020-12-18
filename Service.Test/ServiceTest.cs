using System; 
using System.Linq;
using Domain;
using NUnit.Framework;

namespace Service.Test
{
    public class ServiceTest
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

            var weatherForecastController = new WeatherForecastService();
            var actualResult = weatherForecastController.Get();

            Assert.AreEqual(1, actualResult.Count());
            var actual = actualResult.First();
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Summary, actual.Summary);
            Assert.AreEqual(expected.Temperature, actual.Temperature);
        }
    }
}