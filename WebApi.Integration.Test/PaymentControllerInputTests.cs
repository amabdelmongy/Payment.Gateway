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

using WebApi.Controllers;
using Domain;
using Domain.AcquiringBank;

namespace WebApi.Integration.Test
{
    public class PaymentControllerInputTests
    {
        private readonly HttpClient _client;

        Guid _acquiringBankId = Guid.Parse("fc05a938-ac01-4090-aa1c-34721c1e3346");
         
        public PaymentControllerInputTests()
        {
            var acquiringBankRepository =
                new InMemoryAcquiringBankRepository()
                    .WithId(_acquiringBankId);

            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            IPaymentEventRepository paymentEventRepository = new InMemoryPaymentEventRepository(); 
                            services.AddScoped(a => paymentEventRepository);
                            services.AddScoped(a =>(IAcquiringBankRepository) acquiringBankRepository);
                        });
                    });

            _client = factory.CreateClient();
        }

        [Test]
        public async Task WHEN_PaymentRequestDto_is_correct_THEN_return_OK()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
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

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new
            {
                acquiringBankId = "",
                merchantId = "",
                paymentId = ""
            };
                
            var output = JsonConvert.DeserializeAnonymousType(result, outputDefinition); 
            Assert.AreEqual(paymentRequestDto.MerchantId.ToString(), output.merchantId);
            Assert.AreEqual(_acquiringBankId.ToString(), output.acquiringBankId);
        }

        [Test]
        public async Task WHEN_card_number_empty_THEN_return_error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "",
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

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode );

            var result = await response.Content.ReadAsStringAsync();

            var outputErrorDefinition = new []
            {
                new
                {
                    subject = "",
                    message = ""
                }
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputErrorDefinition);
            Assert.AreEqual("Card Number", output[0].subject);
            Assert.AreEqual("Card Number is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_card_Expiry_empty_THEN_return_error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "348865529252519",
                    Expiry = "",
                    Cvv = "123"
                },
                MerchantId = Guid.Parse("77d17eb6-a996-4375-bf1c-fb9808d95801"),
                Amount = new PaymentController.MoneyDto
                {
                    Currency = "Euro",
                    Value = 10.30
                }
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
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
            Assert.AreEqual("Card Expiry", output[0].subject);
            Assert.AreEqual("Card Expiry is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_card_Cvv_empty_THEN_return_error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "348865529252519",
                    Expiry = "10/24",
                    Cvv = ""
                },
                MerchantId = Guid.Parse("77d17eb6-a996-4375-bf1c-fb9808d95801"),
                Amount = new PaymentController.MoneyDto
                {
                    Currency = "Euro",
                    Value = 10.30
                }
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
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
            Assert.AreEqual("Card Cvv", output[0].subject);
            Assert.AreEqual("Card Cvv is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_Amount_Value_is_lessTHan_0_THEN_return_error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
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
                    Value = -1
                }
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
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
            Assert.AreEqual("Amount value", output[0].subject);
            Assert.AreEqual("Amount value is less than 0", output[0].message);
        }

        [Test]
        public async Task WHEN_Amount_Currency_empty_THEN_return_error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
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
                    Currency = "",
                    Value = 10.30
                }
            };

            var response =
                await _client.PostAsJsonAsync(
                    "/api/payment/request-process-payment/",
                    paymentRequestDto);
             
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
            Assert.AreEqual("Amount Currency", output[0].subject);
            Assert.AreEqual("Amount Currency is Empty", output[0].message);
        }
    }
}