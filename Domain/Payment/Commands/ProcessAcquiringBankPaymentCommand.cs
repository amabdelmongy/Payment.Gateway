using System;

namespace Domain.Payment.Commands
{
    public class ProcessAcquiringBankPaymentCommand : PaymentCommand
    {
        public ProcessAcquiringBankPaymentCommand(
            Guid paymentId
        )
            : base(paymentId)
        {
        }
    }
}
