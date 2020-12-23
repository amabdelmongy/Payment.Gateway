using System;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentFailedEvent : Event
    {
        public AcquiringBankPaymentFailedEvent(
            Guid aggregateId,
            DateTime timeStamp,
            int version,
            Guid? acquiringBankId,
            string details
        )
            : base(
                aggregateId,
                timeStamp,
                version,
                typeof(AcquiringBankPaymentFailedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;
            Details = details;
        }

        public Guid? AcquiringBankId { get; }

        public string Details { get; } 
    }
}
