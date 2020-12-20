using System;

namespace Domain.Payment.Commands
{
    public class FailPaymentAtAcquiringBankCommand : PaymentCommand
    {
        public FailPaymentAtAcquiringBankCommand(Guid paymentId, Guid acquiringBankId, string details) : base(paymentId)
        {
            AcquiringBankId = acquiringBankId;
            Details = details;
        } 
        public Guid AcquiringBankId { get;}
        public string Details { get; }
    }
}
