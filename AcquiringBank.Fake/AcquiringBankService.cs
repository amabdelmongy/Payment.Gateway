using System;
using System.Collections.Generic;
using System.Text;

namespace AcquiringBank.Fake
{ 
    public class AcquiringBankService
    {
        public AcquiringBankPaymentStatus ProcessPayment(AcquiringBankPaymentRequest acquiringBankPaymentRequest)
        {

            return AcquiringBankPaymentStatus.Accepted;
        }
    }
}
