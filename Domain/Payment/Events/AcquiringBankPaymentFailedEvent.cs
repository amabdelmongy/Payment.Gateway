using System;
using Domain.Payment.Aggregate;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentFailedEvent : Event
    {
        public AcquiringBankPaymentFailedEvent(
            Guid aggregateId,
            DateTime timeStamp,
            int version,
            Guid? acquiringBankId,
            string details, 
            PaymentStatus paymentStatus
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
            PaymentStatus = paymentStatus;
        }

        public Guid? AcquiringBankId { get; }

        public string Details { get; } 

        public PaymentStatus PaymentStatus { get; }
    }
}
