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

namespace WebApi.Integration.Test
{
    public class PaymentControllerAcquiringBankStatusTests
    { 
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
        public async Task WHEN_ProcessPayment_return_Exception_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom(
                "Failed calling Acquiring Bank",
                new Exception("Test Exception"));

            Result <Guid> expectedResult = Result.Failed<Guid>(expectedError);

            var client = CreateClient(expectedResult);
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
        public async Task WHEN_ProcessPayment_return_Rejected_THEN_return_Error()
        {
            var acquiringBankId = TestStubs.TestAcquiringBankId;
            var expectedError = Error.CreateFrom(
                $"Rejected to acquiring Bank with Payment Id {acquiringBankId}",
                "Card is not valid");
                   
            Result<Guid> expectedResult = Result.Failed<Guid>(expectedError);
              
            var client = CreateClient(expectedResult);
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
        public async Task WHEN_ProcessPayment_return_RejectedAcquiringBankError_THEN_return_Error()
        {
            var acquiringBankId = TestStubs.TestAcquiringBankId;
            var expectedError = RejectedAcquiringBankError.CreateFrom(
                $"Rejected to acquiring Bank with Payment Id {acquiringBankId}",
                "Card is not valid");

            Result<Guid> expectedResult = Result.Failed<Guid>(expectedError);

            var client = CreateClient(expectedResult);
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
        public async Task WHEN_PaymentRequestDto_is_correct_THEN_return_OK()
        {
            var acquiringBankId = TestStubs.TestAcquiringBankId;
            var client = CreateClient(Result.Ok<Guid>(acquiringBankId));

            var response =
                await client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    TestStubs.TestPaymentRequestDto);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new
            {
                acquiringBankId = "",
                merchantId = "",
                paymentId = ""
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputDefinition);
            Assert.AreEqual(TestStubs.TestPaymentRequestDto.MerchantId.ToString(), output.merchantId);
            Assert.AreEqual(acquiringBankId.ToString(), output.acquiringBankId);
        }
    }
}