using System;
using System.Collections.Generic;
using Domain.Payment.Aggregate;
using Domain.Payment.Events;

namespace Domain.Test
{
    static class PaymentStubsTests
    {
        public static readonly Guid PaymentIdTest = Guid.Parse("977d39b4-2a34-4223-baac-8496ff2a7bc1");

        public static readonly Card CardTest = new Card("5105105105105100", "10/22", "321");

        public static readonly Guid MerchantIdTest = Guid.Parse("4e90ec63-254f-4064-98ea-53516b843d0f");

        public static readonly Guid AcquiringBankIdTest = Guid.Parse("ea637ef8-1aa4-49d8-aa7e-fc57ff82b319");

        public static readonly Money AmountTest = new Money(100.5, "Euro");

        public static PaymentAggregate PaymentAggregateTest()
        {
            var paymentAggregate = PaymentAggregateFactory.CreateFrom(
                new List<Event>
                {
                    new PaymentRequestedEvent(
                        PaymentIdTest, 
                        DateTime.Now, 
                        1, 
                        CardTest, 
                        MerchantIdTest, 
                        AmountTest,
                        PaymentStatus.ProcessStarted
                    )
                }
            );
            return paymentAggregate.Value;
        } 

        public static readonly PaymentRequestedEvent PaymentRequestedEventTest =
            new PaymentRequestedEvent(
                PaymentStubsTests.PaymentIdTest,
                DateTime.Now,
                1,
                PaymentStubsTests.CardTest,
                PaymentStubsTests.MerchantIdTest,
                PaymentStubsTests.AmountTest,
                PaymentStatus.ProcessStarted
            );

        public static readonly AcquiringBankPaymentFailedEvent AcquiringBankPaymentFailedEventTest =
            new AcquiringBankPaymentFailedEvent(
                PaymentStubsTests.PaymentIdTest,
                DateTime.Now,
                1,
                PaymentStubsTests.AcquiringBankIdTest,
                "details",
                PaymentStatus.Failed
            );

        public static readonly AcquiringBankPaymentProcessedEvent AcquiringBankPaymentProcessedEventTest =
            new AcquiringBankPaymentProcessedEvent(
                PaymentStubsTests.PaymentIdTest,
                DateTime.Now,
                1,
                PaymentStubsTests.AcquiringBankIdTest,
                PaymentStatus.Processed
            );
    }
}
