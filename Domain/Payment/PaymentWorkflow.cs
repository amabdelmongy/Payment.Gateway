using System;
using System.Collections.Generic;
using System.Linq;
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
            var processAcquiringBankPaymentCommandResult =
                _paymentCommandHandler.Handle(
                    new ProcessAcquiringBankPaymentCommand(paymentId));

            if (processAcquiringBankPaymentCommandResult.HasErrors)
            {
                var errors = new List<Error>();
                errors.AddRange(processAcquiringBankPaymentCommandResult.Errors);

                var rejectedAcquiringBankErrors =
                    processAcquiringBankPaymentCommandResult.Errors
                        .OfType<RejectedAcquiringBankError>()
                        .Select(rejectedAcquiringBankResult =>
                            _paymentCommandHandler.Handle(
                                new FailAcquiringBankPaymentCommand(
                                    paymentId,
                                    rejectedAcquiringBankResult.AcquiringBankResultId,
                                    rejectedAcquiringBankResult.Message)
                            )
                        );

                var genericErrors =
                    processAcquiringBankPaymentCommandResult.Errors
                        .Where(t => !(t is RejectedAcquiringBankError))
                        .Select(error =>
                            _paymentCommandHandler.Handle(
                                new FailAcquiringBankPaymentCommand(
                                    paymentId,
                                    null,
                                    error.Message)
                            )
                        );

                errors.AddRange(rejectedAcquiringBankErrors.SelectMany(t => t.Errors).ToList());
                errors.AddRange(genericErrors.SelectMany(t => t.Errors).ToList());

                return Result.Failed<Event>(errors);
            }

            return Result.Ok(processAcquiringBankPaymentCommandResult.Value);
        }
    }
}
