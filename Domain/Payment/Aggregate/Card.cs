namespace Domain.Payment.Aggregate
{
    public class Card
    {
        public Card(
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