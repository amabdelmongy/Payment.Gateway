using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Payment.Events;

namespace Domain.Payment
{
    public class PaymentAggregate
    {
        public PaymentAggregate()
        {

        }

        public PaymentAggregate(Guid id, Card card, Guid merchantId, Money amount, int version)
        {
            PaymentId = id;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
            Version = version;
        }

        public Guid PaymentId { get; private set; }

        public Card Card { get; private set; }

        public Guid MerchantId { get; private set; }

        public Money Amount { get; private set; }

        public int Version { get; private set; }

        public PaymentAggregate With(Guid id, Card card, Guid merchantId, Money amount, int version)
        {
            return new PaymentAggregate(id, card, merchantId, amount, version);
        }
    }

    public static class PaymentAggregateFactory
    {
        public static Result<PaymentAggregate> CreateFrom(IEnumerable<Event> events)
        {
            try
            {
                var resultCampaign =
                    events
                        .OrderBy(x => x.Version)
                        .ToList()
                        .Aggregate(new PaymentAggregate(), (paymentAggregate, e) =>
                        {
                            switch (e)
                            {
                                case PaymentProcessCreated @event:
                                    paymentAggregate =
                                        paymentAggregate.With(

                                            @event.AggregateId,
                                            @event.Card,
                                            @event.MerchantId,
                                            @event.Amount,
                                            @event.Version);
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }

                            return paymentAggregate;
                        });

                return Result.Ok(resultCampaign);
            }
            catch (Exception e)
            {
                return Result.Failed<PaymentAggregate>(Error.CreateFrom("CreateCampaign", e));
            }
        }
    }
}