using System;
using Domain.Payment;
using Domain.Payment.Commands;

namespace Domain.AcquiringBank
{ 
    public interface  IAcquiringBankRepository
    {
        Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate);
    }
}
