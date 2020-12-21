using System; 
using System.Linq; 
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;
using Moq;
using NUnit.Framework; 

namespace Domain.Test
{
    public class CommandHandlerFailAcquiringBankPaymentTests
    {

        [Test]
        public void WHEN_handle_RequestPaymentCommand_THEN_create_PaymentRequestedEvent_and_return_Ok()
        {
            var paymentEventRepository = new Mock<IPaymentEventRepository>();
            paymentEventRepository
                    .Setup(repository => 
                        repository.Add(It.IsAny<Event>())
                        )
                    .Returns(Result.Ok<object>());

            var commandHandler = new FailAcquiringBankPaymentCommandHandler(paymentEventRepository.Object);
            var command = new FailAcquiringBankPaymentCommand(
                PaymentStubs.PaymentIdTest,
                PaymentStubs.AcquiringBankIdTest,
                "Error message"
            );

            var expectedVersion = 1; 
            var actualResult = 
                commandHandler.Handle(
                    command ,
                    expectedVersion);

            Assert.IsTrue(actualResult.IsOk);
            var actualEvent = (AcquiringBankPaymentFailedEvent)actualResult.Value;
            Assert.AreEqual("AcquiringBankPaymentFailedEvent", actualEvent.Type);
            Assert.AreEqual(command.PaymentId, actualEvent.AggregateId);
            Assert.AreEqual(command.AcquiringBankId, actualEvent.AcquiringBankId);
            Assert.AreEqual(command.Details, actualEvent.Details); 

            Assert.AreEqual(expectedVersion + 1, actualEvent.Version);
            paymentEventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void WHEN_Handle_RequestPaymentCommand_and_paymentEventRepository_return_error_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject",
                    "Error Message");

            var paymentEventRepository =
                new Mock<IPaymentEventRepository>();

            paymentEventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                    )
                .Returns(Result.Failed<object>(expectedError));

            var commandHandler = new FailAcquiringBankPaymentCommandHandler(paymentEventRepository.Object);
            var command = new FailAcquiringBankPaymentCommand(
                PaymentStubs.PaymentIdTest,
                PaymentStubs.AcquiringBankIdTest,
                "Error message"
            );

            var expectedVersion = 1;
            var actualResult =
                commandHandler.Handle(
                    command,
                    expectedVersion);
             
            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());
            var error = actualResult.Errors.First();

            Assert.AreEqual(expectedError.Subject, error.Subject);
            Assert.AreEqual(expectedError.Message, error.Message);

            paymentEventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }
    }
}