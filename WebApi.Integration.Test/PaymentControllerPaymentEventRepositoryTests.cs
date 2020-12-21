using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http; 
using System.Net.Http.Json; 
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection; 
using Newtonsoft.Json;
using NUnit.Framework;
using Domain.AcquiringBank;
using Domain;
using Domain.Payment.Events;

namespace WebApi.Integration.Test
{
    public class PaymentControllerPaymentEventRepositoryTests
    {  
        private HttpClient CreateClient(IPaymentEventRepository paymentEventRepository)
        { 
            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            ; 
                            services.AddScoped(a => paymentEventRepository);
                            services.AddScoped(a => (IAcquiringBankRepository)new InMemoryAcquiringBankRepository().WithId(TestStubs.TestAcquiringBankId));
                        });
                    });

            return factory.CreateClient();
        }

        [Test]
        public async Task WHEN_Get_return_Exception_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom(
                "Failed calling Data base",
                new Exception("Test Exception PaymentEventRepository"));

            Result<IEnumerable<Event>> expectedResult = Result.Failed<IEnumerable<Event>>(expectedError);

            var paymentEventRepository = new InMemoryPaymentEventRepository().WithNewGet(expectedResult);

            var client = CreateClient((IPaymentEventRepository)paymentEventRepository);
            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    TestStubs.TestPaymentRequestDto);

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
                new Exception("Test Exception PaymentEventRepository"));

            Result<object> expectedResult = Result.Failed<object>(expectedError);

            var paymentEventRepository = new InMemoryPaymentEventRepository().WithNewAdd(expectedResult);

            var client = CreateClient((IPaymentEventRepository)paymentEventRepository);
            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    TestStubs.TestPaymentRequestDto);

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