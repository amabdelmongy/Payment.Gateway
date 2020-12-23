using System;
using Domain.Payment.Aggregate;

namespace Domain.AcquiringBank
{ 
    public interface IAcquiringBankFacade
    {
        Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate);
    }
}
