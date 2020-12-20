using System;
using AcquiringBank.Fake;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;

namespace Data
{
    public class AcquiringBankRepository : IAcquiringBankRepository
    {
        private readonly IAcquiringBankService _acquiringBankService;

        public AcquiringBankRepository(IAcquiringBankService acquiringBankService)
        {
            _acquiringBankService = acquiringBankService;
        }

        public AcquiringBankRepository()
        {
            _acquiringBankService = new AcquiringBankService();
        }

        public Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate)
        {
            try
            { 
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

                var processPayment =
                    _acquiringBankService.ProcessPayment(
                        acquiringBankPaymentRequest);

                return processPayment.PaymentStatus switch
                {
                    AcquiringBankPaymentStatus.Accepted
                        =>
                        Result.Ok<Guid>(processPayment.AcquiringBankPaymentId),

                    AcquiringBankPaymentStatus.Rejected
                        =>
                        Result.Failed<Guid>(
                            RejectedAcquiringBankError.CreateFrom(
                                processPayment.AcquiringBankPaymentId,
                                $"Rejected to acquiring Bank with Payment Id { processPayment.AcquiringBankPaymentId }",
                                processPayment.Details
                            )
                        ),
                    _ => throw new NotImplementedException()
                };
            }
            catch (Exception e)
            {
                return Result.Failed<Guid>(
                    Error.CreateFrom(
                        "Failed calling Acquiring Bank",
                        e
                    ));
            }
        }
    }
}
