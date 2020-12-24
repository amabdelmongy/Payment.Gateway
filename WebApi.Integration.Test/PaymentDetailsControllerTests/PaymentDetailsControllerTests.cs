using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain;
using Domain.Payment.Aggregate;
using Domain.Payment.Events;
using Domain.Payment.Projection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using WebApi.Integration.Test.PaymentControllerTests;

namespace WebApi.Integration.Test.PaymentDetailsControllerTests
{
    public class PaymentDetailsControllerTests
    {
        private const string UrlPaymentGet = "/api/v1/payment-details/";

        private HttpClient CreateClient(List<PaymentProjection> paymentProjections)
        {
            var inMemoryPaymentProjectionRepository = new InMemoryPaymentProjectionRepository();
            paymentProjections.ForEach(paymentProjection =>
                inMemoryPaymentProjectionRepository.Add(paymentProjection)
            );

            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddScoped(a =>
                                (IPaymentProjectionRepository) inMemoryPaymentProjectionRepository);
                        });
                    });

            return factory.CreateClient();
        }

        [Test]
        public async Task WHEN_Get_by_payment_id_THEN_return_correct_Payment()
        {
            var expectPaymentProjection = new PaymentProjection
            {
                PaymentId = Guid.Parse("f00526d2-cfe6-4a34-8ea4-f54ccc00ae7d"),
                AcquiringBankId = Guid.Parse("0380df44-600a-43c1-b354-76fb59c61250"),
                MerchantId = Guid.Parse("ce4b6c04-a63e-4556-8655-601bf8f573d7"),
                CardNumber = "5105105105105100",
                CardExpiry = "10/24",
                CardCvv = "321",
                Currency = "Euro",
                Amount = 10,
                PaymentStatus = PaymentStatus.Processed.Id,
                LastUpdatedDate = DateTime.Now
            };
            var expectedCardNumber = "510510xxxxxx5100";
            var expectedCardExpiry = "1xxxx";
            var expectedCardCvv = "3xx";

            var client = CreateClient(
                new List<PaymentProjection>()
                {
                    expectPaymentProjection
                });

            var response =
                await client.GetAsync(
                    UrlPaymentGet + expectPaymentProjection.PaymentId);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new
            {
                paymentId = "",
                MerchantId = "",
                CardNumber = "",
                CardExpiry = "",
                CardCvv = "",
                Amount = "",
                Currency = "",
                PaymentStatus = "",
                LastUpdatedDate = "",
                AcquiringBankId = ""
            };

            var output = JsonConvert.DeserializeAnonymousType(result, outputDefinition);
            Assert.AreEqual(expectPaymentProjection.MerchantId.ToString(), output.MerchantId);
            Assert.AreEqual(expectPaymentProjection.PaymentId.ToString(), output.paymentId);
            Assert.AreEqual(expectPaymentProjection.AcquiringBankId.ToString(), output.AcquiringBankId);
            Assert.AreEqual(expectedCardNumber, output.CardNumber);
            Assert.AreEqual(expectedCardExpiry, output.CardExpiry);
            Assert.AreEqual(expectedCardCvv, output.CardCvv);
            Assert.AreEqual(expectPaymentProjection.PaymentStatus, output.PaymentStatus);
            Assert.AreEqual(expectPaymentProjection.Amount.ToString(), output.Amount);
            Assert.AreEqual(expectPaymentProjection.Currency, output.Currency);
            Assert.AreEqual(expectPaymentProjection.LastUpdatedDate, DateTime.Parse(output.LastUpdatedDate));
        }


        [Test]
        public async Task WHEN_Get_by_merchant_id_THEN_return_correct_Payment()
        {
            var expectPaymentProjection1 = new PaymentProjection
            {
                PaymentId = Guid.Parse("f00526d2-cfe6-4a34-8ea4-f54ccc00ae7d"),
                AcquiringBankId = Guid.Parse("0380df44-600a-43c1-b354-76fb59c61250"),
                MerchantId = Guid.Parse("ce4b6c04-a63e-4556-8655-601bf8f573d7"),
                CardNumber = "5105105105105100",
                CardCvv = "321",
                Currency = "Euro",
                Amount = 10,
                PaymentStatus = PaymentStatus.Processed.Id,
                CardExpiry = "10/24",
                LastUpdatedDate = DateTime.Now
            };
            var expectPaymentProjection2 = new PaymentProjection
            {
                PaymentId = Guid.Parse("5c0605ec-af80-4818-8808-e604bb05ee2f"),
                AcquiringBankId = Guid.Parse("0380df44-600a-43c1-b354-76fb59c61250"),
                MerchantId = Guid.Parse("ce4b6c04-a63e-4556-8655-601bf8f573d7"),
                CardNumber = "5105105105105100",
                CardCvv = "321",
                Currency = "Euro",
                Amount = 20,
                PaymentStatus = PaymentStatus.Processed.Id,
                CardExpiry = "10/24",
                LastUpdatedDate = DateTime.Now
            }; 
            var expectedCardNumber = "510510xxxxxx5100";
            var expectedCardExpiry = "1xxxx";
            var expectedCardCvv = "3xx";
            var expectPaymentProjections = new List<PaymentProjection>()
            {
                expectPaymentProjection1,
                expectPaymentProjection2
            };
            var client = CreateClient(expectPaymentProjections);

            var response =
                await client.GetAsync(
                    UrlPaymentGet + expectPaymentProjection1.MerchantId + "/get-by-merchant-id");

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            var outputDefinition = new[]
            {
                new
                {
                    paymentId = "",
                    MerchantId = "",
                    CardNumber = "",
                    CardExpiry = "",
                    CardCvv = "",
                    Amount = "",
                    Currency = "",
                    PaymentStatus = "",
                    LastUpdatedDate = "",
                    AcquiringBankId = ""
                }
            };

            var actualList = JsonConvert.DeserializeAnonymousType(result, outputDefinition);

            Assert.AreEqual(2, actualList.Count());

            for (int i = 0; i < expectPaymentProjections.Count; i++)
            {
                var output = actualList[i];
                var expectPaymentProjection =
                    expectPaymentProjections.First(t => t.PaymentId.ToString() == output.paymentId);

                Assert.AreEqual(expectPaymentProjection.MerchantId.ToString(), output.MerchantId);
                Assert.AreEqual(expectPaymentProjection.PaymentId.ToString(), output.paymentId);
                Assert.AreEqual(expectPaymentProjection.AcquiringBankId.ToString(), output.AcquiringBankId);
                Assert.AreEqual(expectedCardNumber, output.CardNumber);
                Assert.AreEqual(expectedCardExpiry, output.CardExpiry);
                Assert.AreEqual(expectedCardCvv, output.CardCvv);
                Assert.AreEqual(expectPaymentProjection.PaymentStatus, output.PaymentStatus);
                Assert.AreEqual(expectPaymentProjection.Amount.ToString(), output.Amount);
                Assert.AreEqual(expectPaymentProjection.Currency, output.Currency);
                Assert.AreEqual(expectPaymentProjection.LastUpdatedDate, DateTime.Parse(output.LastUpdatedDate));
            }
        }

        [Test]
        public async Task WHEN_Repository_return_Exception_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom(
                "Failed calling Data base",
                new Exception("Test Exception from PaymentProjection Repository"));

            Result<PaymentProjection> expectedResult = Result.Failed<PaymentProjection>(expectedError);

            var inMemoryPaymentProjectionRepository = new InMemoryPaymentProjectionRepository();
            inMemoryPaymentProjectionRepository.WithNewGetResult(expectedResult);

            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddScoped(a =>
                                (IPaymentProjectionRepository) inMemoryPaymentProjectionRepository);
                        });
                    });

            var client = factory.CreateClient();

            var response =
                await client.GetAsync(
                    UrlPaymentGet + Guid.NewGuid());


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
        public async Task WHEN_Repository_Get_Merchant_return_Exception_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Failed calling Data base",
                    new Exception("Test Exception from Get_Merchant at PaymentProjection Repository")
                );

            var expectedResult =
                Result.Failed<IEnumerable<PaymentProjection>>(expectedError);
            var inMemoryPaymentProjectionRepository = new InMemoryPaymentProjectionRepository();
            inMemoryPaymentProjectionRepository.WithNewGetByMerchantIdResult(expectedResult);

            var factory =
                new WebApplicationFactory<Startup>()
                    .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureTestServices(services =>
                        {
                            services.AddScoped(a =>
                                (IPaymentProjectionRepository) inMemoryPaymentProjectionRepository);
                        });
                    });

            var client = factory.CreateClient();

            var response =
                await client.GetAsync(
                    UrlPaymentGet + Guid.NewGuid() + "/get-by-merchant-id");


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