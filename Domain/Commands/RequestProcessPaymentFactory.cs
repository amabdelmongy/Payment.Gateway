using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Commands
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
