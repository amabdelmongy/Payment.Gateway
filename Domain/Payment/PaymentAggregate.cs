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

        public PaymentAggregate(
            Guid id,
            Card card,
            Guid merchantId,
            Money amount,
            int version,
            PaymentStatus paymentStatus,
            Guid? acquiringBankId = null)
        {
            PaymentId = id;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
            Version = version;
            PaymentStatus = paymentStatus;
            AcquiringBankId = acquiringBankId;
        }

        public Guid PaymentId { get; }

        public Card Card { get; }

        public Guid MerchantId { get; }

        public Money Amount { get; }

        public int Version { get; }

        public PaymentStatus PaymentStatus { get; }

        public Guid? AcquiringBankId { get; }

        public PaymentAggregate With(
            Guid id,
            Card card,
            Guid merchantId,
            Money amount,
            int version)
        {
            return
                new PaymentAggregate(
                    id,
                    card,
                    merchantId,
                    amount,
                    version,
                    PaymentStatus.ProcessStarted);
        }

        public PaymentAggregate With(
            PaymentStatus paymentStatus,
            int version,
            Guid? acquiringBankId = null)
        {
            return
                new PaymentAggregate(
                    PaymentId,
                    Card,
                    MerchantId,
                    Amount,
                    version,
                    paymentStatus,
                    acquiringBankId);
        }
    }

    public class PaymentStatus
    {
        public static readonly PaymentStatus ProcessStarted =
            new PaymentStatus("processing");

        public static readonly PaymentStatus Processed =
            new PaymentStatus("processed");

        public static readonly PaymentStatus Failed =
            new PaymentStatus("failed");

        private PaymentStatus(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }

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