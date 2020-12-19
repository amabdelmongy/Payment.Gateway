using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Payment
{
    public interface IPayments
    {
        Result<PaymentAggregate> Get(Guid paymentId);
    }

    public class Payments : IPayments
    {
        private readonly IPaymentEventRepository _events;

        public Payments(IPaymentEventRepository events)
        {
            _events = events;
        }

        public Result<PaymentAggregate> Get(Guid paymentId)
        { 
            var events = _events.Get(paymentId);
            if (!events.IsOk)
                return Result.Failed<PaymentAggregate>(events.Errors);

            if (!events.Value.Any()) return Result.Failed<PaymentAggregate>(Error.CreateFrom("PaymentAggregate"));

            return PaymentAggregateFactory.CreateFrom(events.Value);
        } 
    }
}
