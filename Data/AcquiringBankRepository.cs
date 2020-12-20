using System;
using AcquiringBank.Fake;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;

namespace Data
{
    public class AcquiringBankRepository : IAcquiringBankRepository
    {
        public Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate)
        {
            var acquiringBankService = new AcquiringBankService();

            var acquiringBankPaymentRequest = new AcquiringBankPaymentRequest
            (
                paymentAggregate.MerchantId,
                new AcquiringBankMoney(
                    paymentAggregate.Amount.Value,
                    paymentAggregate.Amount.Currency),
                new AcquiringBankCard(
                    paymentAggregate.Card.Number,
                    paymentAggregate.Card.Expiry,
                    paymentAggregate.Card.Cvv)
            );

            var processPayment = acquiringBankService.ProcessPayment(acquiringBankPaymentRequest);

            return processPayment.PaymentStatus switch
            {
                AcquiringBankPaymentStatus.Accepted
                    =>
                    Result.Ok<Guid>(processPayment.AcquiringBankPaymentId),

                AcquiringBankPaymentStatus.Rejected
                    =>
                    Result.Failed<Guid>(
                        Error.CreateFrom(
                            processPayment.AcquiringBankPaymentId + "",
                            processPayment.Details
                        )
                    ),
                _ => throw new NotImplementedException()
            };

        }
    }
}
