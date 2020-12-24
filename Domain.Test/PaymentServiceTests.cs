using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Payment;
using Domain.Payment.Events;
using Moq;
using NUnit.Framework;

namespace Domain.Test
{
    public class PaymentServiceTests
    {
        [Test]
        public void WHEN_eventRepository_has_error_THEN_return_Aggregate()
        {
            var expectedEvent = PaymentStubsTests.PaymentRequestedEventTest;
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock
                .Setup(t =>
                    t.Get(It.IsAny<Guid>())
                )
                .Returns(Result.Ok<IEnumerable<Event>>(
                    new List<Event>()
                    {
                        expectedEvent
                    })
                );

            var paymentService =
                new PaymentService(eventRepositoryMock.Object);

            var actual =
                paymentService.Get(PaymentStubsTests.PaymentIdTest);

            Assert.True(actual.IsOk);
            Assert.AreEqual(expectedEvent.AggregateId, actual.Value.PaymentId); 
        }

        [Test]
        public void WHEN_eventRepository_has_error_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom("subject", "message");
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock
                .Setup(t =>
                    t.Get(It.IsAny<Guid>())
                )
                .Returns(Result.Failed<IEnumerable<Event>>(expectedError));

            var paymentService = 
                new PaymentService(eventRepositoryMock.Object);

            var actual = 
                paymentService.Get(PaymentStubsTests.PaymentIdTest);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expectedError.Subject, actual.Errors.First().Subject);
            Assert.AreEqual(expectedError.Message, actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_eventRepository_has_no_event_THEN_return_Error()
        {
            var eventRepositoryMock = new Mock<IEventRepository>();
            eventRepositoryMock
                .Setup(t =>
                    t.Get(It.IsAny<Guid>())
                )
                .Returns(Result.Ok<IEnumerable<Event>>(new List<Event>()));

            var paymentService =
                new PaymentService(eventRepositoryMock.Object);

            var actual =
                paymentService.Get(PaymentStubsTests.PaymentIdTest);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual($"No PaymentAggregate with paymentId: {PaymentStubsTests.PaymentIdTest}", actual.Errors.First().Subject);
        }
    }
}
