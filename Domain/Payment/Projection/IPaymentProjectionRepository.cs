using System;
using System.Collections.Generic;
using Domain.Payment.Events;

namespace Domain.Payment.Projection
{
    public interface IPaymentProjectionRepository
    {
        Result<IEnumerable<PaymentProjection>> Get(Guid id);
        Result<object> Add(PaymentProjection paymentProjection);
        Result<object> Update(AcquiringBankPaymentProcessedEvent acquiringBankPaymentProcessedEvent);
        Result<object> Update(AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent);
    }
}