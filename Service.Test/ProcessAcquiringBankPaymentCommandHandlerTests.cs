using System;
using System.Collections.Generic;
using System.Linq;
using Domain.AcquiringBank;
using Domain.Payment;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;
using Moq;
using NUnit.Framework; 

namespace Domain.Test
{
    public class ProcessAcquiringBankPaymentCommandHandlerTests
    {
        [Test]
        public void
            GIVEN_ProcessAcquiringBankPaymentCommandHandler_WHEN_Handle_processAcquiringBankPaymentCommand_THEN_Create_AcquiringBankPaymentProcessedEvent_Event_and_return_Ok()
        {
            var paymentEventRepository = new Mock<IPaymentEventRepository>();
            paymentEventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                )
                .Returns(Result.Ok<object>());

            var acquiringBankRepository = new Mock<IAcquiringBankRepository>();
            acquiringBankRepository
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Ok<Guid>(PaymentStubs.AcquiringBankIdTest));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                paymentEventRepository.Object,
                acquiringBankRepository.Object
            );

            var command = new ProcessAcquiringBankPaymentCommand(
                PaymentStubs.PaymentIdTest
            );
            var paymentAggregate = PaymentStubs.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    command
                );

            Assert.IsTrue(actualResult.IsOk);
            var actualEvent = (AcquiringBankPaymentProcessedEvent) actualResult.Value;
            Assert.AreEqual("AcquiringBankPaymentFailedEvent", actualEvent.Type);
            Assert.AreEqual(command.PaymentId, actualEvent.AggregateId);

            Assert.AreEqual(paymentAggregate.Version + 1, actualEvent.Version);
            paymentEventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void GIVEN_ProcessAcquiringBankPaymentCommandHandler_And_paymentEventRepository_return_error_WHEN_Handle_ProcessAcquiringBankPaymentCommand_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject paymentEventRepository",
                    "Error Message paymentEventRepository");

            var paymentEventRepository = new Mock<IPaymentEventRepository>();

            paymentEventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                    )
                .Returns(Result.Failed<object>(expectedError));

            var acquiringBankRepository = new Mock<IAcquiringBankRepository>();
            acquiringBankRepository
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Ok<Guid>(PaymentStubs.AcquiringBankIdTest));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                paymentEventRepository.Object,
                acquiringBankRepository.Object
            );

            var command = new ProcessAcquiringBankPaymentCommand(
                PaymentStubs.PaymentIdTest
            );
            var paymentAggregate = PaymentStubs.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    command
                );

            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());
            var error = actualResult.Errors.First();

            Assert.AreEqual(expectedError.Subject, error.Subject);
            Assert.AreEqual(expectedError.Message, error.Message);

            paymentEventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void GIVEN_ProcessAcquiringBankPaymentCommandHandler_And_acquiringBankRepository_return_error_WHEN_Handle_ProcessAcquiringBankPaymentCommand_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject AcquiringBankRepository",
                    "Error Message AcquiringBankRepository");

            var paymentEventRepository =
                new Mock<IPaymentEventRepository>();

            paymentEventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                )
                .Returns(Result.Ok<object>());


            var acquiringBankRepository = new Mock<IAcquiringBankRepository>();
            acquiringBankRepository
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Failed<Guid>(expectedError));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                paymentEventRepository.Object,
                acquiringBankRepository.Object
            );

            var command = new ProcessAcquiringBankPaymentCommand(
                PaymentStubs.PaymentIdTest
            );
            var paymentAggregate = PaymentStubs.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    command
                );

            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());
            var error = actualResult.Errors.First();

            Assert.AreEqual(expectedError.Subject, error.Subject);
            Assert.AreEqual(expectedError.Message, error.Message); 
        }
    }
}