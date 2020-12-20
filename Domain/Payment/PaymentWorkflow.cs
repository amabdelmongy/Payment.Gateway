using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain.AcquiringBank;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;

namespace Domain.Payment
{
    public interface IPaymentWorkflow
    {
        Result<Event> Run(Guid paymentId);
    }

    public class PaymentWorkflow : IPaymentWorkflow
    {
        private readonly IPaymentCommandHandler _paymentCommandHandler;

        public PaymentWorkflow(IPaymentCommandHandler paymentCommandHandler)
        {
            _paymentCommandHandler = paymentCommandHandler;
        }

        public Result<Event> Run(Guid paymentId)
        {
            var processPaymentAtAcquiringBankStarted =
                _paymentCommandHandler.Handle(
                    new StartProcessPaymentAtAcquiringBank(paymentId));

            if (processPaymentAtAcquiringBankStarted.HasErrors)
                return Result.Failed<Event>(processPaymentAtAcquiringBankStarted.Errors);

            var processPaymentAtAcquiringBankResult =
                _paymentCommandHandler.Handle(
                    new ProcessPaymentAtAcquiringBank(paymentId));

            if (processPaymentAtAcquiringBankResult.HasErrors)
            {
                var errors = new List<Error>();
                errors.AddRange(processPaymentAtAcquiringBankResult.Errors);

                var rejectedAcquiringBankErrors =
                    processPaymentAtAcquiringBankResult.Errors
                        .OfType<RejectedAcquiringBankError>()
                        .Select(rejectedAcquiringBankResult =>
                            _paymentCommandHandler.Handle(
                                new FailPaymentAtAcquiringBankCommand(
                                    paymentId,
                                    rejectedAcquiringBankResult.AcquiringBankResultId,
                                    rejectedAcquiringBankResult.Message)
                            )
                        );

                var genericErrors =
                    processPaymentAtAcquiringBankResult.Errors
                        .Where(t => !(t is RejectedAcquiringBankError))
                        .Select(error =>
                            _paymentCommandHandler.Handle(
                                new FailPaymentAtAcquiringBankCommand(
                                    paymentId,
                                    null,
                                    error.Message)
                            )
                        );

                errors.AddRange(rejectedAcquiringBankErrors.SelectMany(t => t.Errors).ToList());
                errors.AddRange(genericErrors.SelectMany(t => t.Errors).ToList());

                return Result.Failed<Event>(errors);
            }

            return Result.Ok(processPaymentAtAcquiringBankResult.Value);
        }
    }
}
