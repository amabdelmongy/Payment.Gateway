using System;
using System.Linq;
using Domain.AcquiringBank;
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
            var eventRepository = new Mock<IEventRepository>();
            eventRepository
                    .Setup(repository => 
                        repository.Add(It.IsAny<Event>())
                        )
                    .Returns(Result.Ok<object>());

            var commandHandler = new FailAcquiringBankPaymentCommandHandler(eventRepository.Object);
            var command = new FailAcquiringBankPaymentCommand(
                PaymentStubsTests.PaymentIdTest,
                PaymentStubsTests.AcquiringBankIdTest,
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
            eventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void WHEN_Handle_RequestPaymentCommand_and_eventRepository_return_error_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject",
                    "Error Message");

            var eventRepository =
                new Mock<IEventRepository>();

            eventRepository
                .Setup(repository =>
                    repository.Add(It.IsAny<Event>())
                    )
                .Returns(Result.Failed<object>(expectedError));

            var commandHandler = new FailAcquiringBankPaymentCommandHandler(eventRepository.Object);
            var command = new FailAcquiringBankPaymentCommand(
                PaymentStubsTests.PaymentIdTest,
                PaymentStubsTests.AcquiringBankIdTest,
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

            eventRepository.Verify(mock => mock.Add(It.IsAny<Event>()), Times.Once());
        }

        [Test]
        public void WHEN_create_RejectedAcquiringBankError_THEN_return_correct_Object()
        {
            var expectedAcquiringBankId = PaymentStubsTests.AcquiringBankIdTest;
            var expectedSubject = "subject";
            var expectedMessage = "message";

            var rejectedAcquiringBankError =
                RejectedAcquiringBankError.CreateFrom(
                    expectedAcquiringBankId,
                    expectedSubject,
                    expectedMessage
                );


            Assert.AreEqual(expectedSubject, rejectedAcquiringBankError.Subject);
            Assert.AreEqual(expectedMessage, rejectedAcquiringBankError.Message);
            Assert.AreEqual(expectedAcquiringBankId, rejectedAcquiringBankError.AcquiringBankResultId);
        }

        [Test]
        public void WHEN_create_RejectedAcquiringBankError_with_exception_THEN_return_correct_Object()
        { 
            var expectedSubject = "subject";
            var expectedException = new NotImplementedException();

            var rejectedAcquiringBankError =
                RejectedAcquiringBankError.CreateFrom( 
                    expectedSubject,
                    expectedException 
                );


            Assert.AreEqual(expectedSubject, rejectedAcquiringBankError.Subject);
            Assert.AreEqual(expectedException, rejectedAcquiringBankError.Exception);
        }
    }
}