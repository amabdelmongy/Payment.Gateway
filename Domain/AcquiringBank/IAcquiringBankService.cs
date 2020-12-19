using System;
using Domain.Payment;
using Domain.Payment.Commands;

namespace Domain.AcquiringBank
{
    public interface IAcquiringBankRepository
    {
        Result<Object> ProcessPayment(Guid merchantId, Money amount, Card card);
    }
}
