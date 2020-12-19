using System;

namespace Domain.Payment.Commands
{
    public class StartProcessPaymentAtAcquiringBank : PaymentCommand
    {
        public StartProcessPaymentAtAcquiringBank(Guid paymentId) : base(paymentId)
        {
        }
    }
}
