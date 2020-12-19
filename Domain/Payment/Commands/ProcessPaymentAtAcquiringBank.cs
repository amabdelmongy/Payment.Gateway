using System;

namespace Domain.Payment.Commands
{
    public class ProcessPaymentAtAcquiringBank : PaymentCommand
    {
        public ProcessPaymentAtAcquiringBank(Guid paymentId, Card card, Guid merchantId, Money amount)
        {
            PaymentId = paymentId;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Guid PaymentId { get; set; }
        public Card Card { get; private set; } 
        public Guid MerchantId { get; private set; }
        public Money Amount { get; private set; }

    }
}
