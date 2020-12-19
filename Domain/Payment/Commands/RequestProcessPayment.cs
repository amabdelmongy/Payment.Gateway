using System;

namespace Domain.Payment.Commands
{
    public class RequestProcessPayment : PaymentCommand
    {
        public RequestProcessPayment(Card card, Guid merchantId, Money amount)
        {
            PaymentId = Guid.NewGuid();
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
