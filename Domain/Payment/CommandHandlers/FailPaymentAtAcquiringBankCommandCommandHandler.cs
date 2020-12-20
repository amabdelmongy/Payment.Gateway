using System;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IFailAcquiringBankPaymentCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            FailAcquiringBankPaymentCommand failAcquiringBankPaymentCommand);
    }

    public class FailAcquiringBankPaymentCommandHandler : IFailAcquiringBankPaymentCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;

        public FailAcquiringBankPaymentCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
        }

        public Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            FailAcquiringBankPaymentCommand failAcquiringBankPaymentCommand)
        {
            var acquiringBankPaymentFailedEvent =
                new AcquiringBankPaymentFailedEvent(
                    failAcquiringBankPaymentCommand.PaymentId,
                    DateTime.Now,
                    paymentAggregate.Version + 1,
                    failAcquiringBankPaymentCommand.AcquiringBankId,
                    failAcquiringBankPaymentCommand.Details
                );

            var result = _paymentEventRepository.Add(acquiringBankPaymentFailedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) acquiringBankPaymentFailedEvent)
                    : Result.Failed<Event>(result.Errors);
        }
    }
}