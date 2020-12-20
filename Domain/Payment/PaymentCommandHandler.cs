using System;
using Domain.AcquiringBank;
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
        private readonly IAcquiringBankRepository _acquiringBankRepository;

        public PaymentCommandHandler(
            IPaymentEventRepository paymentEventRepository, 
            IPayments payments,
            IAcquiringBankRepository acquiringBankRepository)
        {
            _paymentEventRepository = paymentEventRepository;
            _payments = payments;
            _acquiringBankRepository = acquiringBankRepository;
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
            var processPaymentAtAcquiringBankStartedEvent =
                new ProcessPaymentAtAcquiringBankStartedEvent(
                    startProcessPaymentAtAcquiringBank.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1
                );

            var result = _paymentEventRepository.Add(processPaymentAtAcquiringBankStartedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) processPaymentAtAcquiringBankStartedEvent)
                    : Result.Failed<Event>(result.Errors);
        }


        private Result<Event> Handle(
            PaymentAggregate paymentAggregate, 
            ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank)
        {
            var acquiringBankResult = 
                _acquiringBankRepository.ProcessPayment(
                    paymentAggregate 
                );

            if (acquiringBankResult.HasErrors)
                return Result.Failed<Event>(acquiringBankResult.Errors);

            var paymentAtAcquiringBankProcessedEvent =
                new PaymentAtAcquiringBankProcessedEvent(
                    processPaymentAtAcquiringBank.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1,
                    acquiringBankResult.Value
                );

            var result = 
                _paymentEventRepository
                    .Add(paymentAtAcquiringBankProcessedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event)paymentAtAcquiringBankProcessedEvent)
                    : Result.Failed<Event>(result.Errors);
        }

        private Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            FailPaymentAtAcquiringBankCommand failPaymentAtAcquiringBankCommand)
        {
            var paymentAtAcquiringBankFailedEvent =
                new PaymentAtAcquiringBankFailedEvent(
                    failPaymentAtAcquiringBankCommand.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1,
                    failPaymentAtAcquiringBankCommand.AcquiringBankId,
                    failPaymentAtAcquiringBankCommand.Details
                );

            var result = _paymentEventRepository.Add(paymentAtAcquiringBankFailedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event)paymentAtAcquiringBankFailedEvent)
                    : Result.Failed<Event>(result.Errors);
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
                    => Handle(requestProcessPaymentCommand),

                StartProcessPaymentAtAcquiringBank startProcessPaymentAtAcquiringBank
                    => Handle(paymentResult.Value,
                    startProcessPaymentAtAcquiringBank),

                ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank 
                    => Handle(paymentResult.Value,
                    processPaymentAtAcquiringBank),
                     
                FailPaymentAtAcquiringBankCommand failPaymentAtAcquiringBankCommand
                    => Handle(paymentResult.Value,
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
