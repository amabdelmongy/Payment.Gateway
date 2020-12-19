using System;
using AcquiringBank.Fake;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment.Commands;

namespace Data
{ 
    public class AcquiringBankRepository : IAcquiringBankRepository
    {
        public Result<Object> ProcessPayment(ProcessPaymentAtAcquiringBank payment)
        {
            var acquiringBankService = new AcquiringBankService();

            var acquiringBankPaymentRequest = new AcquiringBankPaymentRequest
            (
                payment.MerchantId, 
                new AcquiringBankMoney(payment.Amount.Value, payment.Amount.Currency),
                new AcquiringBankCard(payment.Card.Number, payment.Card.Expiry, payment.Card.Cvv)
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
