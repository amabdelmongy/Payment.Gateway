using System.Threading.Tasks;
using Domain.Payment.Events;
using Domain.Payment.Projection;

namespace Domain.MessageBus
{
    public interface IMessageBusHandler
    {
        Task Handle(Event paymentEvent);
    }

    public class MessageBusHandler : IMessageBusHandler
    {
        private readonly IPaymentProjectionRepository _paymentProjectionRepository;

        public MessageBusHandler(IPaymentProjectionRepository paymentProjectionRepository)
        {
            _paymentProjectionRepository = paymentProjectionRepository;
        }

        public async Task Handle(Event paymentEvent)
        {
            switch (paymentEvent)
            {
                case PaymentRequestedEvent paymentRequestedEvent:
                    Handel(paymentRequestedEvent);
                    break;
                case AcquiringBankPaymentProcessedEvent paymentProcessedEvent:
                    Handel(paymentProcessedEvent);
                    break;

                case AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent:
                    Handel(acquiringBankPaymentFailedEvent);
                    break;
            }
        }

        private async void Handel(PaymentRequestedEvent paymentRequestedEvent)
        {
            PaymentProjection paymentProjection = new PaymentProjection
            {
                PaymentId = paymentRequestedEvent.AggregateId,
                MerchantId = paymentRequestedEvent.MerchantId,
                CardNumber = paymentRequestedEvent.Card.Number,
                CardExpiry = paymentRequestedEvent.Card.Expiry,
                CardCvv = paymentRequestedEvent.Card.Cvv,
                Currency = paymentRequestedEvent.Amount.Currency,
                Amount = paymentRequestedEvent.Amount.Value,
                PaymentStatus = paymentRequestedEvent.PaymentStatus.Id,
                LastUpdatedDate = paymentRequestedEvent.TimeStamp
            };
            _paymentProjectionRepository.Add(paymentProjection);
        }

        private async void Handel(AcquiringBankPaymentProcessedEvent acquiringBankPaymentProcessed)
        {
            _paymentProjectionRepository.Update(acquiringBankPaymentProcessed);
        }
        private async void Handel(AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent)
        {
            _paymentProjectionRepository.Update(acquiringBankPaymentFailedEvent);
        }
    }
}
