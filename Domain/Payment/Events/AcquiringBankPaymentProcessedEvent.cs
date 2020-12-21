using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentProcessedEvent : Event
    {
        public AcquiringBankPaymentProcessedEvent(
            Guid paymentId,
            DateTime timeStamp,
            int version,
            Guid acquiringBankId
        )
            : base(
                paymentId,
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
