namespace Domain.Payment
{
    public class Money
    {
        public Money(double value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public double Value { get; }
        public string Currency { get; }
    }
}
