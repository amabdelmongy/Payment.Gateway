using System;

namespace Domain.Payment.Commands
{
    public class RequestProcessPaymentFactory
    {
        private readonly IRequestProcessPaymentInputValidator _requestProcessPaymentInputValidator;

        public RequestProcessPaymentFactory(IRequestProcessPaymentInputValidator requestProcessPaymentInputValidator)
        {
            _requestProcessPaymentInputValidator = requestProcessPaymentInputValidator;
        }

        public Result<RequestProcessPayment> From(
            Card card,
            Guid merchantId,
            Money amount)
        {

            var validateStatus = _requestProcessPaymentInputValidator.Validate(card, merchantId, amount);
            return validateStatus.IsOk
                ? Result.Ok(new RequestProcessPayment(card, merchantId, amount))
                : validateStatus;
        }
    }
}
