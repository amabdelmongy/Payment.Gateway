using System; 
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
using WebApi.Controllers; 

namespace WebApi.Integration.Test
{
    public class PaymentControllerAcquiringBankStatusTests
    {
        Guid _acquiringBankId = Guid.Parse("fc05a938-ac01-4090-aa1c-34721c1e3346");
        readonly PaymentController.PaymentRequestDto _paymentRequestDto = new PaymentController.PaymentRequestDto
        {
            Card = new PaymentController.CardDto
            {
                Number = "348865529252519",
                Expiry = "10/24",
                Cvv = "123"
            },
            MerchantId = Guid.Parse("77d17eb6-a996-4375-bf1c-fb9808d95801"),
            Amount = new PaymentController.MoneyDto
            {
                Currency = "Euro",
                Value = 10.30
            }
        };

        private HttpClient CreateClient(Result<Guid> result)
        {
            var acquiringBankRepository =
                new InMemoryAcquiringBankRepository()
                    .WithNewResult(result); 
            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            IPaymentEventRepository paymentEventRepository = new InMemoryPaymentEventRepository(); 
                            services.AddScoped(a => paymentEventRepository);
                            services.AddScoped(a => (IAcquiringBankRepository) acquiringBankRepository);
                        });
                    });

            return factory.CreateClient();
        }

        [Test]
        public async Task WHEN_ProcessPayment_return_Exception_THEN_return_error()
        {
            var expectedError = Error.CreateFrom(
                "Failed calling Acquiring Bank",
                new Exception("Test Exception"));

            Result <Guid> expectedResult = Result.Failed<Guid>(expectedError);

            var client = CreateClient(expectedResult);
            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    _paymentRequestDto);

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
        public async Task WHEN_ProcessPayment_return_Rejected_THEN_return_error()
        { 
            var expectedError = Error.CreateFrom(
                $"Rejected to acquiring Bank with Payment Id {_acquiringBankId}",
                "Card is not valid");
                   
            Result<Guid> expectedResult = Result.Failed<Guid>(expectedError);
              
            var client = CreateClient(expectedResult);
            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    _paymentRequestDto);

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
            var client = CreateClient(Result.Ok<Guid>(_acquiringBankId));

            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    _paymentRequestDto);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new
            {
                acquiringBankId = "",
                merchantId = "",
                paymentId = ""
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputDefinition);
            Assert.AreEqual(_paymentRequestDto.MerchantId.ToString(), output.merchantId);
            Assert.AreEqual(_acquiringBankId.ToString(), output.acquiringBankId);
        }
    }
}