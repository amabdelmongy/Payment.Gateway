namespace AcquiringBank.Fake
{
    public class AcquiringBankMoney
    {
        public AcquiringBankMoney(double value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public double Value { get; }
        public string Currency { get; }
    }
}
