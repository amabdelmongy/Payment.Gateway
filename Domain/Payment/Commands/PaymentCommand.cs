using System;

namespace Domain.Payment.Commands
{
    public abstract class PaymentCommand
    {
        public Guid PaymentId { get; }

        protected PaymentCommand(Guid paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
