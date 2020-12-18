﻿using System;
using System.Collections.Generic;
using System.Text;
using Domain.Commands;

namespace Domain
{
    public interface IHandlePaymentCommands
    {
        Result<Event> Handle(PaymentCommand command);
    }

    public class PaymentCommandHandler : IHandlePaymentCommands
    {
        private Result<Event> Handle(RequestProcessPaymentCommand requestProcessPaymentCommand)
        {
            return Result.Ok<Event>();
        }

        public Result<Event> Handle(PaymentCommand command)
        { 
            return command switch
            {
                RequestProcessPaymentCommand requestProcessPaymentCommand
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
