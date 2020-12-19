using System;

namespace Domain.Payment.Commands
{
    public class RequestProcessPaymentFactory
    {
        public Result<RequestProcessPayment> From(
            Card card,
            Guid merchantId,
            Money amount)
        {
            return
                Result.Ok(
                    new RequestProcessPayment(card, merchantId, amount)
                );
        }
    }
}
