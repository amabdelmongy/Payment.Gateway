using System;

namespace WebApi.dto
{
    public class MoneyDto
    {
        public double Value { get; set; }
        public string Currency { get; set; }
    }

    public class CardDto
    {
        public string Number { get; set; }
        public string Expiry { get; set; }
        public string Cvv { get; set; }
    }

    public class PaymentRequestDto
    {
        public Guid MerchantId { get; set; }
        public MoneyDto Amount { get; set; }
        public CardDto Card { get; set; }
    }
}
