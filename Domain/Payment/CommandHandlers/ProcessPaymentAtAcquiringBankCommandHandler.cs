using System;
using System.Collections.Generic;
using System.Text;
using Domain.AcquiringBank;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IProcessPaymentAtAcquiringBankCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            ProcessPaymentAtAcquiringBank processPaymentAtAcquiringBank);
    }

    public class ProcessPaymentAtAcquiringBankCommandHandler : IProcessPaymentAtAcquiringBankCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;
        private readonly IAcquiringBankRepository _acquiringBankRepository;

        public ProcessPaymentAtAcquiringBankCommandHandler(IPaymentEventRepository paymentEventRepository, IAcquiringBankRepository acquiringBankRepository)
        {
            _paymentEventRepository = paymentEventRepository;
            _acquiringBankRepository = acquiringBankRepository;
        }

        public Result<Event> Handle(
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

    }
}
