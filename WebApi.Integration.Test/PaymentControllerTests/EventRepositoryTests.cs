using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment.Events;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace WebApi.Integration.Test.PaymentControllerTests
{
    public class EventRepositoryTests
    {
        private const string UrlRequestPayment = "/api/v1/payment/request-payment/";

        private HttpClient CreateClient(IEventRepository eventRepository)
        {
            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddScoped(a => eventRepository);
                            services.AddScoped(a =>
                                (IAcquiringBankFacade) new InMemoryAcquiringBankFacade().WithId(PaymentDtoTests
                                    .TestAcquiringBankId));
                        });
                    });

            return factory.CreateClient();
        }

        [Test]
        public async Task WHEN_Get_return_Exception_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Failed calling Data base",
                    new Exception("Test Exception from Event Repository")
                );

            Result<IEnumerable<Event>> expectedResult = Result.Failed<IEnumerable<Event>>(expectedError);

            var eventRepository = new InMemoryEventRepository().WithNewGet(expectedResult);

            var client = CreateClient((IEventRepository)eventRepository);
            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto
                );

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var outputErrorDefinition = new[]
            {
                new
                {
                    subject = "",
                    message = ""
                }
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputErrorDefinition);
            Assert.AreEqual(expectedError.Subject, output[0].subject);
            Assert.AreEqual(expectedError.Message, output[0].message);
        }

        [Test]
        public async Task WHEN_Add_return_Exception_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom(
                "Failed calling Data base",
                new Exception("Test Exception from Event Repository"));

            var expectedResult = Result.Failed<object>(expectedError);

            var eventRepository = new InMemoryEventRepository().WithNewAdd(expectedResult);

            var client = CreateClient((IEventRepository)eventRepository);
            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto
                );

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var result = await response.Content.ReadAsStringAsync();

            var outputErrorDefinition = new[]
            {
                new
                {
                    subject = "",
                    message = ""
                }
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputErrorDefinition);
            Assert.AreEqual(expectedError.Subject, output[0].subject);
            Assert.AreEqual(expectedError.Message, output[0].message);
        }
    }
}