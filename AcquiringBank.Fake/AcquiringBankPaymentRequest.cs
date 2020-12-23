using System;

namespace AcquiringBank.Fake
{
    public class AcquiringBankPaymentRequest
    {

        public AcquiringBankPaymentRequest(Guid merchantId, AcquiringBankMoney amount, AcquiringBankCard card)
        {
            MerchantId = merchantId;
            Amount = amount;
            Card = card;
        } 
        public Guid MerchantId { get; set; }
        public AcquiringBankMoney Amount { get; set; }
        public AcquiringBankCard Card { get; set; }
    }
}
