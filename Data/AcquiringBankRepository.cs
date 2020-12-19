using System;
using AcquiringBank.Fake;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;
using Domain.Payment.Commands;

namespace Data
{
    public class AcquiringBankRepository : IAcquiringBankRepository
    {
        public Result<Object> ProcessPayment(Guid merchantId, Money amount, Card card)
        {
            var acquiringBankService = new AcquiringBankService();

            var acquiringBankPaymentRequest = new AcquiringBankPaymentRequest
            (
                merchantId,
                new AcquiringBankMoney(amount.Value, amount.Currency),
                new AcquiringBankCard(card.Number, card.Expiry, card.Cvv)
            );

            var result = acquiringBankService.ProcessPayment(acquiringBankPaymentRequest);

            switch (result)
            {
                case AcquiringBankPaymentStatus.Accepted:
                    //Todo
                    break;
                case AcquiringBankPaymentStatus.Rejected:
                    // Todo
                    break;
            }

            return Result.Ok<Object>();
        }
    }
}
