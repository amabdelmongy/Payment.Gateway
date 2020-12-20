using System;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IRequestProcessPaymentCommandHandler
    {
        Result<Event> Handle(RequestPaymentCommand requestPaymentCommand);
    }

    public class RequestPaymentCommandHandler : IRequestProcessPaymentCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;

        public RequestPaymentCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
        }

        public Result<Event> Handle(RequestPaymentCommand requestPaymentCommand)
        {
            var paymentRequestedEvent = new PaymentRequestedEvent(
                requestPaymentCommand.PaymentId,
                DateTime.Now,
                1,
                requestPaymentCommand.Card,
                requestPaymentCommand.MerchantId,
                requestPaymentCommand.Amount
            );

            var result = _paymentEventRepository.Add(paymentRequestedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) paymentRequestedEvent)
                    : Result.Failed<Event>(result.Errors);
        }
    }
}
