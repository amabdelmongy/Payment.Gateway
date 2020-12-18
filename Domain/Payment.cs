using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class PaymentAggregate
    {
        public PaymentAggregate(Guid id, Card card, Guid merchantId, Money amount)
        {
            Id = id;
            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Guid Id { get; private set; }

        public Card Card { get; private set; }

        public Guid MerchantId { get; private set; }

        public Money Amount { get; private set; }

    }
}