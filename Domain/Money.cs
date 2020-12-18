using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Money {
        public Money(double value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public double Value { get; private set; }
        public string Currency { get; private set; }
    }
}
