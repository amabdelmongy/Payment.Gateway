using System;
using System.Collections.Generic;
using Domain.Payment.Events;

namespace Domain.Payment.Projection
{
    public interface IPaymentProjectionRepository
    {
        Result<PaymentProjection> Get(Guid id);
        Result<IEnumerable<PaymentProjection>> GetByMerchantId(Guid merchantId);
        Result<object> Add(PaymentProjection paymentProjection);
        Result<object> Update(AcquiringBankPaymentProcessedEvent acquiringBankPaymentProcessedEvent);
        Result<object> Update(AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent);
    }
}