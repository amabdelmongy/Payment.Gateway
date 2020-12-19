using System;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment
{
    public interface IPaymentCommandHandler
    {
        Result<Event> Handle(PaymentCommand command);
    }

    public class PaymentCommandHandler : IPaymentCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;
        private readonly IPayments _payments;

        public PaymentCommandHandler(IPaymentEventRepository paymentEventRepository, IPayments payments)
        {
            _paymentEventRepository = paymentEventRepository;
            _payments = payments;
        }

        private Result<Event> Handle(RequestProcessPayment requestProcessPayment)
        {
            var paymentProcessCreated = new PaymentProcessCreated(
                requestProcessPayment.PaymentId,
                DateTime.Now,
                1,
                requestProcessPayment.Card,
                requestProcessPayment.MerchantId,
                requestProcessPayment.Amount
            );

            var result = _paymentEventRepository.Add(paymentProcessCreated);
             
            return
                result.IsOk
                    ? Result.Ok((Event)paymentProcessCreated)
                    : Result.Failed<Event>(result.Errors);
        }
         
        private Result<Event> Handle( 
            PaymentAggregate paymentAggregate,
            StartProcessPaymentAtAcquiringBank startProcessPaymentAtAcquiringBank)
        {
            var processPaymentAtAcquiringBankStartedEvent = new ProcessPaymentAtAcquiringBankStartedEvent(
                startProcessPaymentAtAcquiringBank.PaymentId,
                DateTime.Now,
                paymentAggregate.Version + 1
            );

            var result = _paymentEventRepository.Add(processPaymentAtAcquiringBankStartedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event)processPaymentAtAcquiringBankStartedEvent)
                    : Result.Failed<Event>(result.Errors);
        }


        private Result<Event> Handle(ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank)
        {
              throw new NotImplementedException(); 
        }

        public Result<Event> Handle(PaymentCommand command)
        {
            var paymentResult = command is RequestProcessPayment
                ? Result.Ok<PaymentAggregate>(null)
                : _payments.Get(command.PaymentId);
            if (paymentResult.HasErrors) return Result.Failed<Event>(paymentResult.Errors);
             
            return command switch
            {
                RequestProcessPayment requestProcessPaymentCommand
                => Handle(requestProcessPaymentCommand),
                StartProcessPaymentAtAcquiringBank startProcessPaymentAtAcquiringBank
                    => Handle(startProcessPaymentAtAcquiringBank),
                ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank
                    => Handle(processPaymentAtAcquiringBank),

                _ => Result.Failed<Event>(
                    Error.CreateFrom(
                        "Payment Command Handler", 
                        "Command not found")
                )

            };
        }
    }
}
