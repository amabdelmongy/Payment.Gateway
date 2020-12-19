namespace AcquiringBank.Fake
{
    public class AcquiringBankMoney {
        public AcquiringBankMoney(double value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public double Value { get; private set; }
        public string Currency { get; private set; }
    }
}
