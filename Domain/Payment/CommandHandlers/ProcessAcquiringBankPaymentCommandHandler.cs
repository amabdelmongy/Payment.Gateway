using System;
using Domain.AcquiringBank;
using Domain.Payment.Aggregate;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IProcessAcquiringBankPaymentCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            ProcessAcquiringBankPaymentCommand processAcquiringBankPaymentCommand
        );
    }

    public class ProcessAcquiringBankPaymentCommandHandler : IProcessAcquiringBankPaymentCommandHandler
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAcquiringBankFacade _acquiringBankFacade;

        public ProcessAcquiringBankPaymentCommandHandler(
            IEventRepository eventRepository,
            IAcquiringBankFacade acquiringBankFacade
        )
        {
            _eventRepository = eventRepository;
            _acquiringBankFacade = acquiringBankFacade;
        }

        public Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            ProcessAcquiringBankPaymentCommand processAcquiringBankPaymentCommand
        )
        {
            var acquiringBankResult =
                _acquiringBankFacade.ProcessPayment(
                    paymentAggregate
                );

            if (acquiringBankResult.HasErrors)
                return Result.Failed<Event>(acquiringBankResult.Errors);

            var acquiringBankPaymentProcessedEvent =
                new AcquiringBankPaymentProcessedEvent(
                    processAcquiringBankPaymentCommand.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1,
                    acquiringBankResult.Value, 
                    PaymentStatus.Processed
                );

            var result =
                _eventRepository
                    .Add(acquiringBankPaymentProcessedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) acquiringBankPaymentProcessedEvent)
                    : Result.Failed<Event>(result.Errors);
        }
    }
}