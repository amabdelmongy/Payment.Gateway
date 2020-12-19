using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class PaymentProcessCreated : Event
    {
        public PaymentProcessCreated (
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
                typeof(PaymentProcessCreated)
                )
        {
            RequestId = requestId;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Guid RequestId { get; set; }

        public Card Card { get; private set; }

        public Guid MerchantId { get; private set; }

        public Money Amount { get; private set; }

        public static PaymentProcessCreated CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<PaymentProcessCreated>(eventData);
        }
    }
}
