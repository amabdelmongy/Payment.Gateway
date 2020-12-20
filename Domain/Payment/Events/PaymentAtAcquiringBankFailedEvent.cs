using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class AcquiringBankPaymentFailedEvent : Event
    {
        public AcquiringBankPaymentFailedEvent(
            Guid requestId,
            DateTime timeStamp,
            int version,
            Guid? acquiringBankId,
            string details)
            : base(
                requestId,
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

        public static AcquiringBankPaymentFailedEvent CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<AcquiringBankPaymentFailedEvent>(eventData);
        }
    }
}
