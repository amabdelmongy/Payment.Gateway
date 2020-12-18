using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Card
    {
        public Card(string number, string expiry, string cvv)
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