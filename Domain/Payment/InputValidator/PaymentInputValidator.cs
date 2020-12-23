using System.Collections.Generic;
using Domain.Payment.Aggregate;

namespace Domain.Payment.InputValidator
{
    public interface IPaymentInputValidator
    {
        Result<object> Validate(Card card, Money amount);
    }

    public class PaymentInputValidator : IPaymentInputValidator
    {   
        public Result<object> Validate(
            Card card,
            Money amount
        )
        {
            var validatedAmountResult = new AmountValidator().Validate(amount);
            var validatedCardResult = new CardValidator().Validate(card);

            if (validatedCardResult.IsOk && 
                validatedAmountResult.IsOk
            )
                return Result.Ok<object>();

            var errors = new List<Error>();
            errors.AddRange(validatedAmountResult.Errors);
            errors.AddRange(validatedCardResult.Errors);
            return Result.Failed<object>(errors);
        }
    }
}

