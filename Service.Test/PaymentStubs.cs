using System;
using System.Collections.Generic;
using System.Text;
using Domain.Payment;
using Domain.Payment.Events;

namespace Domain.Test
{
    static class PaymentStubs
    {
        public static readonly Guid PaymentIdTest = Guid.Parse("977d39b4-2a34-4223-baac-8496ff2a7bc1");

        public static readonly Card CardTest = new Card("5105105105105100","12/24", "321");
        
        public static readonly Guid MerchantIdTest = Guid.Parse("977d39b4-2a34-4223-baac-8496ff2a7bc1");

        public static readonly Guid AcquiringBankIdTest = Guid.Parse("ea637ef8-1aa4-49d8-aa7e-fc57ff82b319");

        public static readonly Money AmountTest = new Money(100.5, "Euro");

        public static PaymentAggregate PaymentAggregateTest ()
        {
            var paymentAggregate = PaymentAggregateFactory.CreateFrom(
                new List<Event>
                {
                    new PaymentRequestedEvent(PaymentIdTest, DateTime.Now, 1, CardTest, MerchantIdTest, AmountTest) 
                }
            );
            return paymentAggregate.Value;
        }

    }
}
