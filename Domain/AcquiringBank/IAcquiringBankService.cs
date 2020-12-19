using System;
using Domain.Payment.Commands;

namespace Domain.AcquiringBank
{
    public interface IAcquiringBankRepository
    {
        Result<Object> ProcessPayment(ProcessPaymentAtAcquiringBank payment);
    }
}
