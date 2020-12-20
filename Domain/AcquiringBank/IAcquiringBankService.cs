using System;
using Domain.Payment;
using Domain.Payment.Commands;

namespace Domain.AcquiringBank
{
    public class RejectedAcquiringBankError : Error
    {
        public RejectedAcquiringBankError(Guid acquiringBankResultId, string subject, string message) : base(subject , null , message)
        {
            AcquiringBankResultId = acquiringBankResultId; 
        }

        public Guid AcquiringBankResultId { get; } 
        public static Error CreateFrom(Guid acquiringBankResultId, string subject, string message = null)
        {
            return new RejectedAcquiringBankError(acquiringBankResultId, subject, message);
        } 
    }

    public interface  IAcquiringBankRepository
    {
        Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate);
    }
}
