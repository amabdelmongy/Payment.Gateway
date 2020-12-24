using System;
using Domain.Payment.Aggregate;
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
            Guid acquiringBankId,
            PaymentStatus paymentStatus
        )
            : base(
                aggregateId,
                timeStamp,
                version,
                typeof(AcquiringBankPaymentProcessedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;
            PaymentStatus = paymentStatus;
        }
          
        public Guid AcquiringBankId { get; } 

        public PaymentStatus PaymentStatus { get; }
    }
}
