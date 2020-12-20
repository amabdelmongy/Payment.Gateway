using Domain.AcquiringBank;
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
        private readonly IStartProcessPaymentAtAcquiringBankCommandHandler _startProcessPaymentAtAcquiringBankCommandHandler;
        private readonly IProcessPaymentAtAcquiringBankCommandHandler _acquiringBankCommandHandler;
        private readonly IFailPaymentAtAcquiringBankCommandHandler _failPaymentAtAcquiringBankCommandHandler;

        public PaymentCommandHandler(
            IPaymentEventRepository paymentEventRepository, 
            IPayments payments,
            IAcquiringBankRepository acquiringBankRepository, 
            IRequestProcessPaymentCommandHandler requestProcessPaymentCommandHandler, 
            IStartProcessPaymentAtAcquiringBankCommandHandler startProcessPaymentAtAcquiringBankCommandHandler,
            IProcessPaymentAtAcquiringBankCommandHandler acquiringBankCommandHandler, 
            IFailPaymentAtAcquiringBankCommandHandler failPaymentAtAcquiringBankCommandHandler)
        {
            _payments = payments;
            _requestProcessPaymentCommandHandler = requestProcessPaymentCommandHandler;
            _startProcessPaymentAtAcquiringBankCommandHandler = startProcessPaymentAtAcquiringBankCommandHandler;
            _failPaymentAtAcquiringBankCommandHandler = failPaymentAtAcquiringBankCommandHandler;
            _acquiringBankCommandHandler = acquiringBankCommandHandler;
        }
      
        public Result<Event> Handle(PaymentCommand command)
        {
            var paymentResult = command is RequestProcessPayment
                ? Result.Ok<PaymentAggregate>(null)
                : _payments.Get(command.PaymentId);
           
            if (paymentResult.HasErrors) 
                return Result.Failed<Event>(paymentResult.Errors);

            return command switch
            {
                RequestProcessPayment requestProcessPaymentCommand 
                    => _requestProcessPaymentCommandHandler.Handle(requestProcessPaymentCommand),

                StartProcessPaymentAtAcquiringBank startProcessPaymentAtAcquiringBank
                    => _startProcessPaymentAtAcquiringBankCommandHandler.Handle(
                        paymentResult.Value,
                        startProcessPaymentAtAcquiringBank),
                
                ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank 
                    => _acquiringBankCommandHandler.Handle(
                        paymentResult.Value,
                        processPaymentAtAcquiringBank),
                     
                FailPaymentAtAcquiringBankCommand failPaymentAtAcquiringBankCommand
                    => _failPaymentAtAcquiringBankCommandHandler.Handle(paymentResult.Value,
                        failPaymentAtAcquiringBankCommand),

                _ => Result.Failed<Event>(
                    Error.CreateFrom(
                        "Payment Command Handler",
                        "Command not found")
                )

            };
        }
    }
}
