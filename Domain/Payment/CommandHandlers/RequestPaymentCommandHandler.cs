﻿using System;
using Domain.Payment.Aggregate;
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
        private readonly IEventRepository _eventRepository;

        public RequestPaymentCommandHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Result<Event> Handle(RequestPaymentCommand requestPaymentCommand)
        {
            var paymentRequestedEvent =
                new PaymentRequestedEvent(
                    requestPaymentCommand.PaymentId,
                    DateTime.Now,
                    1,
                    requestPaymentCommand.Card,
                    requestPaymentCommand.MerchantId,
                    requestPaymentCommand.Amount,
                    PaymentStatus.ProcessStarted
                );

            var result = _eventRepository.Add(paymentRequestedEvent);

            return
                result.IsOk
                    ? Result.Ok((Event) paymentRequestedEvent)
                    : Result.Failed<Event>(result.Errors);
        }
    }
}
