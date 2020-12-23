using System.Collections.Generic;
using Domain.Payment;
using Domain.Payment.Aggregate;
using Domain.Payment.Events;
using NUnit.Framework;

namespace Domain.Test
{
    public class PaymentAggregateFactoryTests
    {
        [Test]
        public void WHEN_pass_PaymentRequestedEvent_THEN_return_correct_Aggregate()
        { 
            var expectedEvent = PaymentStubs.PaymentRequestedEventTest;
            var paymentAggregateResult =
                PaymentAggregateFactory.CreateFrom(
                    new List<Event>
                    {
                        expectedEvent
                    }
                );

            Assert.True(paymentAggregateResult.IsOk);
            var actualAggregate = paymentAggregateResult.Value;

            Assert.AreEqual(expectedEvent.MerchantId, actualAggregate.MerchantId);
            Assert.AreEqual(expectedEvent.AggregateId, actualAggregate.PaymentId);
            Assert.AreEqual(expectedEvent.Card, actualAggregate.Card);
            Assert.AreEqual(expectedEvent.Amount, actualAggregate.Amount);
            Assert.AreEqual(expectedEvent.Version, actualAggregate.Version);
            Assert.AreEqual(PaymentStatus.ProcessStarted.Id, actualAggregate.PaymentStatus.Id);
            Assert.AreEqual(null, actualAggregate.AcquiringBankId);
        }

        [Test]
        public void WHEN_pass_AcquiringBankPaymentFailedEvent_THEN_return_correct_Aggregate()
        {
            var expectedEvent = PaymentStubs.AcquiringBankPaymentFailedEventTest; 

            var paymentAggregateResult = 
                PaymentAggregateFactory.CreateFrom(
                    new List<Event>
                    {
                        PaymentStubs.PaymentRequestedEventTest,
                        expectedEvent
                    }
                );

            Assert.True(paymentAggregateResult.IsOk);
            var actualAggregate = paymentAggregateResult.Value;

            Assert.AreEqual(expectedEvent.Version, actualAggregate.Version);
            Assert.AreEqual(PaymentStatus.Failed.Id, actualAggregate.PaymentStatus.Id);
            Assert.AreEqual(expectedEvent.AcquiringBankId, actualAggregate.AcquiringBankId);
        }

        [Test]
        public void WHEN_pass_AcquiringBankPaymentProcessedEvent_THEN_return_correct_Aggregate()
        {
            var expectedEvent = PaymentStubs.AcquiringBankPaymentProcessedEventTest;


            var paymentAggregateResult =
                PaymentAggregateFactory.CreateFrom(
                    new List<Event>
                    {
                        PaymentStubs.PaymentRequestedEventTest,
                        expectedEvent
                    }
                );

            Assert.True(paymentAggregateResult.IsOk);
            var actualAggregate = paymentAggregateResult.Value;

            Assert.AreEqual(expectedEvent.Version, actualAggregate.Version);
            Assert.AreEqual(PaymentStatus.Processed.Id, actualAggregate.PaymentStatus.Id);
            Assert.AreEqual(expectedEvent.AcquiringBankId, actualAggregate.AcquiringBankId);
        }
    }
}
