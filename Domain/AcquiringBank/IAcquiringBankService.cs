using System;
using Domain.Payment;

namespace Domain.AcquiringBank
{ 
    public interface IAcquiringBankRepository
    {
        Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate);
    }
}
