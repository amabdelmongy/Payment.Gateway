using System;

namespace AcquiringBank.Fake
{
    public interface IAcquiringBankService
    {
        AcquiringBankPaymentResult ProcessPayment(AcquiringBankPaymentRequest acquiringBankPaymentRequest);
    }

    public class AcquiringBankService : IAcquiringBankService
    {
        public AcquiringBankPaymentResult ProcessPayment(
            AcquiringBankPaymentRequest acquiringBankPaymentRequest
        )
        {
            return new Random().Next(0, 3) switch
            {
                0 => AcceptedResult(),
                1 => ExceptionResult(),
                2 => RejectedResultWithCard(),
                3 => RejectedResultWithAmount(),
                _ => AcceptedResult()
            };
        }

        private AcquiringBankPaymentResult AcceptedResult()
        {
            return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Accepted, "Accepted");
        }
        private AcquiringBankPaymentResult ExceptionResult()
        {
            throw new Exception("something modified at Acquiring Bank Service.");
        }

        private AcquiringBankPaymentResult RejectedResultWithCard()
        {
            return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Rejected, "Card is not valid.");
        }

        private AcquiringBankPaymentResult RejectedResultWithAmount()
        {
            return new AcquiringBankPaymentResult(Guid.NewGuid(), AcquiringBankPaymentStatus.Rejected, "Amount is less than the required to transfer.");
        }
    }
}
