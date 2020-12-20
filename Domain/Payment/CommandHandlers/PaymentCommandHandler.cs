using Domain.Payment.Commands;

namespace Domain.Payment.CommandHandlers
{
    public interface IPaymentCommandHandler
    {
        Result<Event> Handle(PaymentCommand command);
    }

    public class PaymentCommandHandler : IPaymentCommandHandler
    {
        private readonly IPayments _payments;
        private readonly IRequestProcessPaymentCommandHandler _requestProcessPaymentCommandHandler;
        private readonly IProcessAcquiringBankPaymentCommandHandler _acquiringBankCommandHandler;
        private readonly IFailAcquiringBankPaymentCommandHandler _failAcquiringBankPaymentCommandHandler;

        public PaymentCommandHandler(
            IPayments payments,
            IRequestProcessPaymentCommandHandler requestProcessPaymentCommandHandler,
            IProcessAcquiringBankPaymentCommandHandler acquiringBankCommandHandler,
            IFailAcquiringBankPaymentCommandHandler failAcquiringBankPaymentCommandHandler)
        {
            _payments = payments;
            _requestProcessPaymentCommandHandler = requestProcessPaymentCommandHandler;
            _failAcquiringBankPaymentCommandHandler = failAcquiringBankPaymentCommandHandler;
            _acquiringBankCommandHandler = acquiringBankCommandHandler;
        }

        public Result<Event> Handle(PaymentCommand command)
        {
            var paymentResult = command is RequestPaymentCommand
                ? Result.Ok<PaymentAggregate>(null)
                : _payments.Get(command.PaymentId);

            if (paymentResult.HasErrors)
                return Result.Failed<Event>(paymentResult.Errors);

            return command switch
            {
                RequestPaymentCommand requestPaymentCommand
                    => _requestProcessPaymentCommandHandler.Handle(requestPaymentCommand),

                ProcessAcquiringBankPaymentCommand processAcquiringBankPaymentCommand
                    => _acquiringBankCommandHandler.Handle(
                        paymentResult.Value,
                        processAcquiringBankPaymentCommand),

                FailAcquiringBankPaymentCommand failAcquiringBankPaymentCommand
                    => _failAcquiringBankPaymentCommandHandler.Handle(paymentResult.Value,
                        failAcquiringBankPaymentCommand),

                _ => Result.Failed<Event>(
                    Error.CreateFrom(
                        "Payment Command Handler",
                        "Command not found")
                )

            };
        }
    }
}
