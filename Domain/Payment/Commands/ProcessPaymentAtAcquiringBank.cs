using System;

namespace Domain.Payment.Commands
{
    public class ProcessPaymentAtAcquiringBank : PaymentCommand
    {
        public ProcessPaymentAtAcquiringBank(Guid paymentId) : base(paymentId)
        { 
        }
    }
}
