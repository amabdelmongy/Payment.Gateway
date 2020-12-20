using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentProcessedEvent : Event
    {
        public AcquiringBankPaymentProcessedEvent(
            Guid requestId,
            DateTime timeStamp,
            int version,
            Guid acquiringBankId
        )
            : base(
                requestId,
                timeStamp,
                version,
                typeof(AcquiringBankPaymentProcessedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;

        }

        public Guid AcquiringBankId { get; }

        public static AcquiringBankPaymentProcessedEvent CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<AcquiringBankPaymentProcessedEvent>(eventData);
        }
    }
}
