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

        Guid _acquiringBankId = TestStubs.TestAcquiringBankId;
         
        public PaymentControllerInputTests()
        {
            var acquiringBankFacade =
                new InMemoryAcquiringBankFacade()
                    .WithId(_acquiringBankId);

            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            IEventRepository eventRepository = new InMemoryEventRepository(); 
                            services.AddScoped(a => eventRepository);
                            services.AddScoped(a =>(IAcquiringBankFacade) acquiringBankFacade);
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
                    Number = "5105105105105100",
                    Expiry = "9/22",
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
        public async Task WHEN_card_number_empty_THEN_return_Error()
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
            Assert.AreEqual("Invalid Card Number", output[0].subject);
            Assert.AreEqual("Card Number is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_card_Expiry_empty_THEN_return_Error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "5105105105105100",
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
            Assert.AreEqual("Invalid Expiry Date", output[0].subject);
            Assert.AreEqual("Expire date is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_card_Cvv_empty_THEN_return_Error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "5105105105105100",
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
            Assert.AreEqual("Invalid CVV", output[0].subject);
            Assert.AreEqual("Card CVV is Empty", output[0].message);
        }

        [Test]
        public async Task WHEN_Amount_Value_is_lessTHan_0_THEN_return_Error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "5105105105105100",
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
            Assert.AreEqual("Invalid Amount", output[0].subject);
            Assert.AreEqual("Amount should be more than 0", output[0].message);
        }

        [Test]
        public async Task WHEN_Amount_Currency_empty_THEN_return_Error()
        {
            var paymentRequestDto = new PaymentController.PaymentRequestDto
            {
                Card = new PaymentController.CardDto
                {
                    Number = "5105105105105100",
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
            Assert.AreEqual("Invalid Amount", output[0].subject);
            Assert.AreEqual("Amount Currency is Empty", output[0].message);
        }
    }
}