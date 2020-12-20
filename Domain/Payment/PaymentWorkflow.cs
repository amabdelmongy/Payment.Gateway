using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var failPaymentAtAcquiringBankCommandResult =
                    _paymentCommandHandler.Handle(
                        new FailPaymentAtAcquiringBankCommand(
                            paymentId,
                            Guid.Parse(processPaymentAtAcquiringBankResult.Errors.First().Subject),
                            processPaymentAtAcquiringBankResult.Errors.First().Message
                        ));

                var errors = new List<Error>();
                errors.AddRange(processPaymentAtAcquiringBankResult.Errors);
                errors.AddRange(failPaymentAtAcquiringBankCommandResult.Errors);

                return Result.Failed<Event>(errors);
            }

            return Result.Ok(processPaymentAtAcquiringBankResult.Value);
        }
    }
}
