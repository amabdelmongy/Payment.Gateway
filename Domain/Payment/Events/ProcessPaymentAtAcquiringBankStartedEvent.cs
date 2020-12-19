using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public class ProcessPaymentAtAcquiringBankStartedEvent : Event
    {
        public ProcessPaymentAtAcquiringBankStartedEvent(
            Guid paymentId,
            DateTime timeStamp,
            int version
        )
            : base(
                paymentId,
                timeStamp,
                version,
                typeof(ProcessPaymentAtAcquiringBankStartedEvent)
                )
        { 
        } 

        public static ProcessPaymentAtAcquiringBankStartedEvent CreateFrom(string eventData)
        {
            return JsonConvert.DeserializeObject<ProcessPaymentAtAcquiringBankStartedEvent>(eventData);
        }
    }
}
