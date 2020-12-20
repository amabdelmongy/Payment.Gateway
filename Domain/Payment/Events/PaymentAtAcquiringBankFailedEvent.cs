using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class PaymentAtAcquiringBankFailedEvent : Event
    {
        public PaymentAtAcquiringBankFailedEvent(
            Guid requestId,
            DateTime timeStamp,
            int version,
            Guid? acquiringBankId,
            string details)
            : base(
                requestId,
                timeStamp,
                version,
                typeof(PaymentAtAcquiringBankFailedEvent)
            )
        {
            AcquiringBankId = acquiringBankId;
            Details = details;
        }

        public Guid? AcquiringBankId { get; set; }

        public string Details { get; set; }

        public static PaymentAtAcquiringBankFailedEvent CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<PaymentAtAcquiringBankFailedEvent>(eventData);
        }
    }
}
