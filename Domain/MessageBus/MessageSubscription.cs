using Domain.Payment.Events;

namespace Domain.MessageBus
{
    public class MessageSubscription
    {
        private readonly IMessageBusHandler _messageBusHandler;
        private readonly IServiceBusPublisher _serviceBusPublishers;

        public MessageSubscription(
            IServiceBusPublisher serviceBusPublishers,
            IMessageBusHandler messageBusHandler
        )
        {
            _serviceBusPublishers = serviceBusPublishers;
            _messageBusHandler = messageBusHandler;
        }
        public void AddSubscriptions()
        {
            _serviceBusPublishers.AddSubscription<PaymentRequestedEvent>(_messageBusHandler.Handle);
            _serviceBusPublishers.AddSubscription<AcquiringBankPaymentFailedEvent>(_messageBusHandler.Handle);
            _serviceBusPublishers.AddSubscription<AcquiringBankPaymentProcessedEvent>(_messageBusHandler.Handle);
        }
    }
}
