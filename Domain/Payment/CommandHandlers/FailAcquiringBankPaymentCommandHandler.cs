using System;
using Domain.Payment.Aggregate;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IFailAcquiringBankPaymentCommandHandler
    {
        Result<Event> Handle(
            FailAcquiringBankPaymentCommand failAcquiringBankPaymentCommand,
            int version
        );
    }
    public class FailAcquiringBankPaymentCommandHandler : IFailAcquiringBankPaymentCommandHandler
    {
        private readonly IEventRepository _eventRepository;

        public FailAcquiringBankPaymentCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public Result<Event> Handle(
            FailAcquiringBankPaymentCommand failAcquiringBankPaymentCommand,
            int version
        )
        {
            var acquiringBankPaymentFailedEvent =
                new AcquiringBankPaymentFailedEvent(
                    failAcquiringBankPaymentCommand.PaymentId,
                    DateTime.Now,
                    version + 1,
                    failAcquiringBankPaymentCommand.AcquiringBankId,
                    failAcquiringBankPaymentCommand.Details,
                    PaymentStatus.Failed
                );

            var result = _eventRepository.Add(acquiringBankPaymentFailedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) acquiringBankPaymentFailedEvent)
                    : Result.Failed<Event>(result.Errors);
        }
    }
}