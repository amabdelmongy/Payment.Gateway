using System;

namespace Domain.Payment.Aggregate
{
    public class PaymentAggregate
    {
        public PaymentAggregate()
        {

        }
        PaymentAggregate(
            Guid id,
            Card card,
            Guid merchantId,
            Money amount,
            int version,
            PaymentStatus paymentStatus,
            Guid? acquiringBankId = null
        )
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
            int version
        )
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
            Guid? acquiringBankId = null
        )
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
}