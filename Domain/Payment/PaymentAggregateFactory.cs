using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Payment.Events;

namespace Domain.Payment
{
    public static class PaymentAggregateFactory
    {
        public static Result<PaymentAggregate> CreateFrom(IEnumerable<Event> events)
        {
            try
            {
                var resultPayment =
                    events
                        .OrderBy(x => x.Version)
                        .ToList()
                        .Aggregate(new PaymentAggregate(), (paymentAggregate, e) =>
                        {
                            switch (e)
                            {
                                case PaymentRequestedEvent @event:
                                    paymentAggregate =
                                        paymentAggregate.With(
                                            @event.AggregateId,
                                            @event.Card,
                                            @event.MerchantId,
                                            @event.Amount,
                                            @event.Version);
                                    break;

                                case AcquiringBankPaymentProcessedEvent @event:
                                    paymentAggregate =
                                        paymentAggregate.With(
                                            PaymentStatus.Processed,
                                            @event.Version,
                                            @event.AcquiringBankId);
                                    break;

                                case AcquiringBankPaymentFailedEvent @event:
                                    paymentAggregate =
                                        paymentAggregate.With(
                                            PaymentStatus.Failed,
                                            @event.Version,
                                            @event.AcquiringBankId);
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }
                            return paymentAggregate;
                        });

                return Result.Ok(resultPayment);
            }
            catch (Exception e)
            {
                return Result.Failed<PaymentAggregate>(Error.CreateFrom("CreatePayment", e));
            }
        }
    }
}
