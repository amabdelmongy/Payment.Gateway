namespace Domain.Payment.Aggregate
{
    public class Money
    {
        public Money(
            double value,
            string currency
        )
        {
            Value = value;
            Currency = currency;
        }

        public double Value { get; }
        public string Currency { get; }
    }
}
