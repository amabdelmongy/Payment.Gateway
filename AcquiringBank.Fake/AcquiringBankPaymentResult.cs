using System;

namespace AcquiringBank.Fake
{
    public class AcquiringBankPaymentResult
    {
        public AcquiringBankPaymentResult(
            Guid acquiringBankPaymentId,
            AcquiringBankPaymentStatus paymentStatus,
            string details)
        {
            AcquiringBankPaymentId = acquiringBankPaymentId;
            PaymentStatus = paymentStatus;
            Details = details;
        }

        public Guid AcquiringBankPaymentId { get; }

        public AcquiringBankPaymentStatus PaymentStatus { get; }
        public string Details { get; }

    }
}
