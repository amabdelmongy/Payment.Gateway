using System;
using System.Collections.Generic;
using System.Text;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IStartProcessPaymentAtAcquiringBankCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate, 
            StartProcessPaymentAtAcquiringBank requestProcessPayment);
    }

    public class StartProcessPaymentAtAcquiringBankCommandHandler : IStartProcessPaymentAtAcquiringBankCommandHandler
    {
        private IPaymentEventRepository _paymentEventRepository;

        public StartProcessPaymentAtAcquiringBankCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
        }

        public Result<Event> Handle(
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
                    ? Result.Ok((Event)processPaymentAtAcquiringBankStartedEvent)
                    : Result.Failed<Event>(result.Errors);
        }

    }
}
