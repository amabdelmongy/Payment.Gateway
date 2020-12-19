using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class PaymentAtAcquiringBankProcessedEvent : Event
    {
        public PaymentAtAcquiringBankProcessedEvent(
            Guid requestId,
            DateTime timeStamp,
            int version,
            Guid acquiringBankId
        )
            : base(
                requestId,
                timeStamp,
                version,
                typeof(PaymentAtAcquiringBankProcessedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;

        }

        public Guid AcquiringBankId { get; set; }

        public static PaymentAtAcquiringBankProcessedEvent CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<PaymentAtAcquiringBankProcessedEvent>(eventData);
        }
    }
}
