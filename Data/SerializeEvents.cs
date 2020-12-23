using System;
using Domain;
using Domain.Payment.Events;
using Newtonsoft.Json;

namespace Data
{
    public static class SerializeEvents
    {
        public static Result<string> SerializeEvent(Event @event)
        {
            string eventData;
            switch (@event)
            {
                case PaymentRequestedEvent paymentRequestedEvent:
                    eventData = JsonConvert.SerializeObject(paymentRequestedEvent);
                    break;

                case AcquiringBankPaymentProcessedEvent acquiringBankPaymentProcessedEvent:
                    eventData = JsonConvert.SerializeObject(acquiringBankPaymentProcessedEvent);
                    break;

                case AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent:
                    eventData = JsonConvert.SerializeObject(acquiringBankPaymentFailedEvent);
                    break;

                default:
                    return Result.Failed<string>(
                        Error.CreateFrom("Serialize Event",
                            $"Not valid event type"));
            }
            return Result.Ok(eventData);
        }

        public static Event DeserializeEvent(
            string eventType,
            string eventData
        )
        {
            return eventType switch
            {
                nameof(PaymentRequestedEvent)
                    => JsonConvert.DeserializeObject<PaymentRequestedEvent>(eventData),

                nameof(AcquiringBankPaymentProcessedEvent)
                    => JsonConvert.DeserializeObject<AcquiringBankPaymentProcessedEvent>(eventData),

                nameof(AcquiringBankPaymentFailedEvent)
                    => JsonConvert.DeserializeObject<AcquiringBankPaymentFailedEvent>(eventData),

                _ => throw new AggregateException($"Couldn't process the event of Type {eventType}'")
            };
        }
    }
}
