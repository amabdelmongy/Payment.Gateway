using System;

namespace AcquiringBank.Fake
{ 
    public class AcquiringBankCard
    {
        public AcquiringBankCard(string number, string expiry, string cvv)
        {
            Number = number;
            Cvv = cvv;
            Expiry = expiry;
        }

        public string Number { get; private set; }
        public string Expiry { get; private set; }
        public string Cvv { get; private set; }
    }
}
