using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class PaymentRequestedEvent : Event
    {
        public PaymentRequestedEvent(
            Guid paymentId,
            DateTime timeStamp,
            int version,
            Card card,
            Guid merchantId,
            Money amount
        )
            : base(
                paymentId,
                timeStamp,
                version,
                typeof(PaymentRequestedEvent)
            )
        {
            PaymentId = paymentId;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Guid PaymentId { get; }

        public Card Card { get; }

        public Guid MerchantId { get; }

        public Money Amount { get; }
         
    }
}
