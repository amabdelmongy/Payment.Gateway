using System;
using System.Collections.Generic;
using System.Text;
using Domain.Commands;
using Domain.Events;

namespace Domain
{
    public interface IHandlePaymentCommands
    {
        Result<Event> Handle(PaymentCommand command);
    }

    public class PaymentCommandHandler : IHandlePaymentCommands
    {
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

            return Result.Ok<Event>();
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
