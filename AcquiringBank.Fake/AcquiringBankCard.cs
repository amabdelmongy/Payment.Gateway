namespace AcquiringBank.Fake
{
    public class AcquiringBankCard
    {
        public AcquiringBankCard(
            string number,
            string expiry,
            string cvv
        )
        {
            Number = number;
            Cvv = cvv;
            Expiry = expiry;
        }

        public string Number { get; }
        public string Expiry { get; }
        public string Cvv { get; }
    }
}
