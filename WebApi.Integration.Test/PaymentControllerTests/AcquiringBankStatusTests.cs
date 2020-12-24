using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using Domain.AcquiringBank;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;

namespace WebApi.Integration.Test.PaymentControllerTests
{
    public class AcquiringBankStatusTests
    {
        private const string UrlRequestPayment = "/api/v1/payment/request-payment/";
        private HttpClient CreateClient(Result<Guid> result)
        {
            var acquiringBankFacade =
                new InMemoryAcquiringBankFacade()
                    .WithNewResult(result); 
            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            IEventRepository eventRepository = new InMemoryEventRepository(); 
                            services.AddScoped(a => eventRepository);
                            services.AddScoped(a => (IAcquiringBankFacade) acquiringBankFacade);
                        });
                    });

            return factory.CreateClient();
        }
        
        [Test]
        public async Task WHEN_ProcessPayment_return_Exception_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Failed calling Acquiring Bank",
                    new Exception("Test Exception")
                );

            var expectedResult = Result.Failed<Guid>(expectedError);

            var client = CreateClient(expectedResult);
            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto);

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
        public async Task WHEN_ProcessPayment_return_Rejected_THEN_return_Error()
        {
            var acquiringBankId = PaymentDtoTests.TestAcquiringBankId;
            var expectedError = 
                Error.CreateFrom(
                $"Rejected to acquiring Bank with Payment Id {acquiringBankId}",
                "Card is not valid"
                );
                   
            var expectedResult = Result.Failed<Guid>(expectedError);
              
            var client = CreateClient(expectedResult);
            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto);

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
        public async Task WHEN_ProcessPayment_return_RejectedAcquiringBankError_THEN_return_Error()
        {
            var acquiringBankId = PaymentDtoTests.TestAcquiringBankId;
            var expectedError =
                RejectedAcquiringBankError.CreateFrom(
                    $"Rejected to acquiring Bank with Payment Id {acquiringBankId}",
                    "Card is not valid"
                );

            var expectedResult = Result.Failed<Guid>(expectedError);

            var client = CreateClient(expectedResult);
            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto);

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
        public async Task WHEN_PaymentRequestDto_is_correct_THEN_return_OK()
        {
            var acquiringBankId = PaymentDtoTests.TestAcquiringBankId;
            var client = CreateClient(Result.Ok<Guid>(acquiringBankId));

            var response =
                await client.PostAsJsonAsync(
                    UrlRequestPayment,
                    PaymentDtoTests.TestPaymentRequestDto
                );

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new
            {
                acquiringBankId = "",
                merchantId = "",
                paymentId = ""
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputDefinition);
            Assert.AreEqual(PaymentDtoTests.TestPaymentRequestDto.MerchantId.ToString(), output.merchantId);
            Assert.AreEqual(acquiringBankId.ToString(), output.acquiringBankId);
        }
    }
}