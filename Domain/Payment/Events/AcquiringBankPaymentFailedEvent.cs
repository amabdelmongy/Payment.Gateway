using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentFailedEvent : Event
    {
        public AcquiringBankPaymentFailedEvent(
            Guid paymentId,
            DateTime timeStamp,
            int version,
            Guid? acquiringBankId,
            string details)
            : base(
                paymentId,
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
