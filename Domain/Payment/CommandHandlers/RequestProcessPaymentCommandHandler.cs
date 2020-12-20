using System;
using Domain.Payment.Commands;
using Domain.Payment.Events;

namespace Domain.Payment.CommandHandlers
{
    public interface IRequestProcessPaymentCommandHandler
    {
        Result<Event> Handle(RequestProcessPayment requestProcessPayment);
    }

    public class RequestProcessPaymentCommandHandler : IRequestProcessPaymentCommandHandler
    {
        private IPaymentEventRepository _paymentEventRepository;

        public RequestProcessPaymentCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
        }

        public Result<Event> Handle(RequestProcessPayment requestProcessPayment)
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
    }
}
