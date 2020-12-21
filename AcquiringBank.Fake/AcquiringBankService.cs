using System;
using System.Collections.Generic;
using System.Text;

namespace AcquiringBank.Fake
{
    public interface IAcquiringBankService
    {
        AcquiringBankPaymentResult ProcessPayment(AcquiringBankPaymentRequest acquiringBankPaymentRequest);
    }

    public class AcquiringBankService : IAcquiringBankService
    {
        public AcquiringBankPaymentResult ProcessPayment(AcquiringBankPaymentRequest acquiringBankPaymentRequest)
        {
            throw new Exception("Some thing modified");
           // return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Accepted, "Accepted");
            //return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Rejected, "Card is not valid");
        }
    }
}
