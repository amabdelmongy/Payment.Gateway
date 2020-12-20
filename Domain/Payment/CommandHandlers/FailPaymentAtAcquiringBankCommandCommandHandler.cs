using System;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IFailPaymentAtAcquiringBankCommandHandler
    {
        Result<Event> Handle(
            PaymentAggregate paymentAggregate,
            FailPaymentAtAcquiringBankCommand failPaymentAtAcquiringBankCommand);
    }

    public class FailPaymentAtAcquiringBankCommandHandler : IFailPaymentAtAcquiringBankCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;

        public FailPaymentAtAcquiringBankCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
        }
        public Result<Event> Handle(
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
    }
}
