using System;
using Domain.AcquiringBank;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IProcessAcquiringBankPaymentCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            ProcessAcquiringBankPaymentCommand processAcquiringBankPaymentCommand);
    }

    public class ProcessAcquiringBankPaymentCommandHandler : IProcessAcquiringBankPaymentCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;
        private readonly IAcquiringBankRepository _acquiringBankRepository;

        public ProcessAcquiringBankPaymentCommandHandler(IPaymentEventRepository paymentEventRepository,
            IAcquiringBankRepository acquiringBankRepository)
        {
            _paymentEventRepository = paymentEventRepository;
            _acquiringBankRepository = acquiringBankRepository;
        }

        public Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            ProcessAcquiringBankPaymentCommand processAcquiringBankPaymentCommand)
        {
            var acquiringBankResult =
                _acquiringBankRepository.ProcessPayment(
                    paymentAggregate
                );

            if (acquiringBankResult.HasErrors)
                return Result.Failed<Event>(acquiringBankResult.Errors);

            var acquiringBankPaymentProcessedEvent =
                new AcquiringBankPaymentProcessedEvent(
                    processAcquiringBankPaymentCommand.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1,
                    acquiringBankResult.Value
                );

            var result =
                _paymentEventRepository
                    .Add(acquiringBankPaymentProcessedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) acquiringBankPaymentProcessedEvent)
                    : Result.Failed<Event>(result.Errors);
        }

    }
}