using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentProcessedEvent : Event
    {
        [JsonConstructor]
        public AcquiringBankPaymentProcessedEvent(
            Guid aggregateId,
            DateTime timeStamp,
            int version,
            Guid acquiringBankId
        )
            : base(
                aggregateId,
                timeStamp,
                version,
                typeof(AcquiringBankPaymentProcessedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;
        }
          
        public Guid AcquiringBankId { get; }
    }
}
