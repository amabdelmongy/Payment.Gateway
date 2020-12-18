using System;

namespace Domain.Commands
{
    public class RequestProcessPayment : PaymentCommand
    {
        public RequestProcessPayment(Card card, Guid merchantId, Money amount)
        {
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }
        
        public Card Card { get; private set; }

        public Guid MerchantId { get; private set; }

        public Money Amount { get; private set; }

    }
}
