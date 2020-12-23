using System;
using System.Linq;
using Domain.Payment.Aggregate;

namespace Domain.Payment
{
    public interface IPaymentService
    {
        Result<PaymentAggregate> Get(Guid paymentId);
    }
    public class PaymentService : IPaymentService
    {
        private readonly IEventRepository _events;

        public PaymentService(IEventRepository events)
        {
            _events = events;
        }

        public Result<PaymentAggregate> Get(Guid paymentId)
        {
            var events = _events.Get(paymentId);
            if (events.HasErrors)
                return Result.Failed<PaymentAggregate>(events.Errors);

            if (!events.Value.Any()) 
                return Result.Failed<PaymentAggregate>(Error.CreateFrom($"No PaymentAggregate with paymentId: { paymentId }"));

            return PaymentAggregateFactory.CreateFrom(events.Value);
        }
    }
}
