using System;
using System.Linq;
using Domain.AcquiringBank;
using Domain.Payment.Aggregate;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;
using Moq;
using NUnit.Framework; 

namespace Domain.Test
{
    public class CommandHandlerProcessAcquiringBankPaymentTests
    {
        [Test]
        public void WHEN_Handle_processAcquiringBankPaymentCommand_THEN_Create_AcquiringBankPaymentProcessedEvent_Event_and_return_Ok()
        {
            var eventRepository = new Mock<IEventRepository>();
            eventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                )
                .Returns(Result.Ok<object>());

            var acquiringBankFacadeMock = new Mock<IAcquiringBankFacade>();
            acquiringBankFacadeMock
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Ok<Guid>(PaymentStubsTests.AcquiringBankIdTest));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                eventRepository.Object,
                acquiringBankFacadeMock.Object
            );

            var expectedCommand = new ProcessAcquiringBankPaymentCommand(
                PaymentStubsTests.PaymentIdTest
            );
            var paymentAggregate = PaymentStubsTests.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    expectedCommand
                );

            Assert.IsTrue(actualResult.IsOk);

            var actualEvent = (AcquiringBankPaymentProcessedEvent) actualResult.Value;
            Assert.AreEqual("AcquiringBankPaymentProcessedEvent", actualEvent.Type);
            Assert.AreEqual(expectedCommand.PaymentId, actualEvent.AggregateId);
            Assert.AreEqual(paymentAggregate.Version + 1, actualEvent.Version);
            eventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void WHEN_Handle_ProcessAcquiringBankPaymentCommand_And_eventRepository_return_error_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject eventRepository",
                    "Error Message eventRepository");

            var eventRepository = new Mock<IEventRepository>();

            eventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                    )
                .Returns(Result.Failed<object>(expectedError));

            var acquiringBankFacadeMock = new Mock<IAcquiringBankFacade>();
            acquiringBankFacadeMock
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Ok<Guid>(PaymentStubsTests.AcquiringBankIdTest));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                eventRepository.Object,
                acquiringBankFacadeMock.Object
            );

            var command = new ProcessAcquiringBankPaymentCommand(
                PaymentStubsTests.PaymentIdTest
            );
            var paymentAggregate = PaymentStubsTests.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    command
                );

            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());
            var actualError = actualResult.Errors.First();

            Assert.AreEqual(expectedError.Subject, actualError.Subject);
            Assert.AreEqual(expectedError.Message, actualError.Message);

            eventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void WHEN_Handle_ProcessAcquiringBankPaymentCommand_And_acquiringBankFacade_return_error_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject AcquiringBankFacade",
                    "Error Message AcquiringBankFacade");

            var eventRepositoryMock = new Mock<IEventRepository>();

            eventRepositoryMock
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                )
                .Returns(Result.Ok<object>());
             
            var acquiringBankFacade = new Mock<IAcquiringBankFacade>();
            acquiringBankFacade
                .Setup(repository =>
                    repository.ProcessPayment(It.IsAny<PaymentAggregate>())
                )
                .Returns(Result.Failed<Guid>(expectedError));

            var commandHandler = new ProcessAcquiringBankPaymentCommandHandler(
                eventRepositoryMock.Object,
                acquiringBankFacade.Object
            );

            var command = new ProcessAcquiringBankPaymentCommand(
                PaymentStubsTests.PaymentIdTest
            );
            var paymentAggregate = PaymentStubsTests.PaymentAggregateTest();

            var actualResult =
                commandHandler.Handle(
                    paymentAggregate,
                    command
                );

            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());

            var actualError = actualResult.Errors.First();
            Assert.AreEqual(expectedError.Subject, actualError.Subject);
            Assert.AreEqual(expectedError.Message, actualError.Message); 
        }
    }
}