using System;
using System.Collections.Generic;
using System.Text;

namespace AcquiringBank.Fake
{ 
    public class AcquiringBankService
    {
        public AcquiringBankPaymentResult ProcessPayment(AcquiringBankPaymentRequest acquiringBankPaymentRequest)
        {
            //return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Accepted, "Accepted");
            return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Rejected, "Card is not valid");
        }
    }
}
