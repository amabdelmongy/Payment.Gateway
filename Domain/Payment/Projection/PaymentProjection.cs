using System;

namespace Domain.Payment.Projection
{
    public class PaymentProjection
    {
        public Guid PaymentId { get; set; }
        public Guid MerchantId { get; set; }
        public string CardNumber { get; set; }
        public string CardExpiry { get; set; }
        public string CardCvv { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public Guid? AcquiringBankId { get; set; }
    }
}
