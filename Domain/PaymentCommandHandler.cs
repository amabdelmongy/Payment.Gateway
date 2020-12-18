using System;
using System.Collections.Generic;
using System.Text;
using Domain.Commands;
using Domain.Events;

namespace Domain
{
    public interface IPaymentCommandHandler
    {
        Result<Event> Handle(PaymentCommand command);
    }

    public class PaymentCommandHandler : IPaymentCommandHandler
    {
        private readonly IPaymentEventRepository _paymentEventRepository;

        public PaymentCommandHandler(IPaymentEventRepository paymentEventRepository)
        {
            _paymentEventRepository = paymentEventRepository;
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

        public Result<Event> Handle(PaymentCommand command)
        { 
            return command switch
            {
                RequestProcessPayment requestProcessPaymentCommand
                => Handle(requestProcessPaymentCommand),
                _ => Result.Failed<Event>(
                    Error.CreateFrom(
                        "Payment Command Handler", 
                        "Command not found")
                )
            };
        }
    }
}
