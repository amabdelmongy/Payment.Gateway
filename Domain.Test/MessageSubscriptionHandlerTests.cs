using System.Threading.Tasks;
using Domain.MessageBus;
using Domain.Payment.Events;
using Domain.Payment.Projection;
using Moq;
using NUnit.Framework;

namespace Domain.Test
{
    class MessageSubscriptionHandlerTests
    {
        [Test]
        public async Task WHEN_pass_PaymentRequestedEvent_THEN_call_PaymentProjectionRepository_Add()
        {
            var paymentRequestedEvent = PaymentStubsTests.PaymentRequestedEventTest;
            var expectedPaymentProjection = new PaymentProjection
            {
                PaymentId = paymentRequestedEvent.AggregateId,
                MerchantId = paymentRequestedEvent.MerchantId,
                CardNumber = paymentRequestedEvent.Card.Number,
                CardExpiry = paymentRequestedEvent.Card.Expiry,
                CardCvv = paymentRequestedEvent.Card.Cvv,
                Currency = paymentRequestedEvent.Amount.Currency,
                Amount = paymentRequestedEvent.Amount.Value,
                PaymentStatus = paymentRequestedEvent.PaymentStatus.Id,
                LastUpdatedDate = paymentRequestedEvent.TimeStamp
            };

            var paymentProjectionRepositoryMock = new Mock<IPaymentProjectionRepository>();
            paymentProjectionRepositoryMock
                .Setup(repository =>
                    repository.Add(expectedPaymentProjection))
                .Returns(Result.Ok<object>());

            var messageBusHandler = new MessageBusHandler(paymentProjectionRepositoryMock.Object);

            var a = messageBusHandler.Handle(paymentRequestedEvent);


            paymentProjectionRepositoryMock.Verify(
                t => t.Add(
                    It.IsAny<PaymentProjection>()
                ), Times.Once);
        }

        [Test]
        public async Task WHEN_pass_AcquiringBankPaymentProcessedEvent_THEN_call_PaymentProjectionRepository_Update()
        {
            var expectedEvent = PaymentStubsTests.AcquiringBankPaymentProcessedEventTest;
            var paymentProjectionRepositoryMock = new Mock<IPaymentProjectionRepository>();
            paymentProjectionRepositoryMock
                .Setup(repository =>
                    repository.Update(It.IsAny<AcquiringBankPaymentProcessedEvent>()))
                .Returns(Result.Ok<object>());

            var messageBusHandler = new MessageBusHandler(paymentProjectionRepositoryMock.Object);

            var a = messageBusHandler.Handle(expectedEvent);

            paymentProjectionRepositoryMock.Verify(
                t => t.Update(
                    expectedEvent
                ), Times.Once
            );
        }

        [Test]
        public async Task WHEN_pass_AcquiringBankPaymentFailedEvent_THEN_call_PaymentProjectionRepository_Update()
        {
            var expectedEvent = PaymentStubsTests.AcquiringBankPaymentFailedEventTest;
            var paymentProjectionRepositoryMock = new Mock<IPaymentProjectionRepository>();
            paymentProjectionRepositoryMock
                .Setup(repository =>
                    repository.Update(It.IsAny<AcquiringBankPaymentFailedEvent>()))
                .Returns(Result.Ok<object>());

            var messageBusHandler = new MessageBusHandler(paymentProjectionRepositoryMock.Object);

            var a = messageBusHandler.Handle(expectedEvent);

            paymentProjectionRepositoryMock.Verify(
                t => t.Update(
                    expectedEvent
                ), Times.Once);
        }
    }
}
