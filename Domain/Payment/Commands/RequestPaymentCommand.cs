using System;
using Domain.Payment.Aggregate;

namespace Domain.Payment.Commands
{
    public class RequestPaymentCommand : PaymentCommand
    {
        public RequestPaymentCommand(
            Card card,
            Guid merchantId,
            Money amount
        )
            : base(Guid.NewGuid())
        {

            Card = card;
            MerchantId = merchantId;
            Amount = amount;
        }

        public Card Card { get; }

        public Guid MerchantId { get; }

        public Money Amount { get; }
    }
}

