using Domain.Payment.Events;

namespace Domain.MessageBus
{
    public class MessageSubscription
    {
        private readonly IHandler _handler;
        private readonly IServiceBusPublisher _serviceBusPublishers;

        public MessageSubscription(
            IServiceBusPublisher serviceBusPublishers,
            IHandler handler)
        {
            _serviceBusPublishers = serviceBusPublishers;
            _handler = handler;
        }
        public void AddSubscriptions()
        {
            _serviceBusPublishers.AddSubscription<PaymentRequestedEvent>(_handler.Handle);
            _serviceBusPublishers.AddSubscription<AcquiringBankPaymentFailedEvent>(_handler.Handle);
            _serviceBusPublishers.AddSubscription<AcquiringBankPaymentProcessedEvent>(_handler.Handle);
        }
    }
}
