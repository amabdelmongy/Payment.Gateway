using System;
using System.Collections.Generic;
using System.Linq;
using Domain.AcquiringBank;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment
{
    public interface IPaymentWorkflow
    {
        Result<Event> Run(Card card, Guid merchantId, Money amount);
    }
    public class PaymentWorkflow : IPaymentWorkflow
    {
        private readonly IPaymentCommandHandler _paymentCommandHandler;
        private readonly IPaymentInputValidator _paymentInputValidator;

        public PaymentWorkflow(
            IPaymentCommandHandler paymentCommandHandler,
            IPaymentInputValidator paymentInputValidator
        )
        {
            _paymentCommandHandler = paymentCommandHandler;
            _paymentInputValidator = paymentInputValidator;
        }

        public Result<Event> Run(Card card, Guid merchantId, Money amount)
        {
            var validateStatus =
                _paymentInputValidator.Validate(
                    card,
                    amount
                );

            if (validateStatus.HasErrors)
                return Result.Failed<Event>(validateStatus.Errors);

            var paymentRequestedEvent =
                _paymentCommandHandler.Handle(
                    new RequestPaymentCommand(card, merchantId, amount)
                );

            if (paymentRequestedEvent.HasErrors)
                return Result.Failed<Event>(paymentRequestedEvent.Errors);

            var paymentId = paymentRequestedEvent.Value.AggregateId;

            var acquiringBankPaymentProcessedEvent =
                _paymentCommandHandler.Handle(
                    new ProcessAcquiringBankPaymentCommand(paymentId));

            if (acquiringBankPaymentProcessedEvent.HasErrors)
            { 
                var paymentErrors =
                    acquiringBankPaymentProcessedEvent.Errors
                        .Select(error =>
                            _paymentCommandHandler.Handle(
                                new FailAcquiringBankPaymentCommand(
                                    paymentId,
                                    error is RejectedAcquiringBankError
                                        ? (Guid?) ((RejectedAcquiringBankError) error).AcquiringBankResultId
                                        : null,
                                    error.Message)
                            )
                        );

                var errors = new List<Error>();
                errors.AddRange(acquiringBankPaymentProcessedEvent.Errors);
                errors.AddRange(paymentErrors.SelectMany(t => t.Errors).ToList());
                return Result.Failed<Event>(errors);
            }

            return acquiringBankPaymentProcessedEvent;
        }
    }
}
