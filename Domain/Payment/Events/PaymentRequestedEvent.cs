using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class PaymentRequestedEvent : Event
    {
        public PaymentRequestedEvent(
            Guid requestId,
            DateTime timeStamp,
            int version,
            Card card,
            Guid merchantId,
            Money amount
        )
            : base(
                requestId,
                timeStamp,
                version,
                typeof(PaymentRequestedEvent)
            )
        {
            RequestId = requestId;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Guid RequestId { get; }

        public Card Card { get; }

        public Guid MerchantId { get; }

        public Money Amount { get; }
         
    }
}
