using System;
using Domain.Payment.Aggregate;

namespace Domain.Payment.Events
{
    public class PaymentRequestedEvent : Event
    {
        public PaymentRequestedEvent(
            Guid aggregateId,
            DateTime timeStamp,
            int version,
            Card card,
            Guid merchantId,
            Money amount,
            PaymentStatus paymentStatus
        )
            : base(
                aggregateId,
                timeStamp,
                version,
                typeof(PaymentRequestedEvent)
            )
        {
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
            PaymentStatus = paymentStatus;
        }

        public Card Card { get; }

        public Guid MerchantId { get; }

        public Money Amount { get; }

        public PaymentStatus PaymentStatus { get; }
    }
}