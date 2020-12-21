using System;
using System.Collections.Generic;
using Domain.Payment.Commands;

namespace Domain.Payment
{
    public interface IPaymentInputValidator
    {
        Result<RequestPaymentCommand> Validate(Card card, Guid merchantId, Money amount);
    }

    public class PaymentInputValidator : IPaymentInputValidator
    {
        private Result<Money> ValidateAmount(Money amount)
        {
            var errors = new List<Error>();
            if (string.IsNullOrEmpty(amount.Currency))
                errors.Add(Error.CreateFrom("Amount Currency", "Amount Currency is Empty"));

            if (amount.Value < 0)
                errors.Add(Error.CreateFrom("Amount value", "Amount value is less than 0"));

            return errors.Count > 0 ? Result.Failed<Money>(errors) : Result.Ok<Money>(amount);
        }
        private Result<Card> ValidateCard(Card card)
        {
            var errors = new List<Error>();
            if (string.IsNullOrEmpty(card.Number))
                errors.Add(Error.CreateFrom("Card Number", "Card Number is Empty"));

            if (string.IsNullOrEmpty(card.Expiry))
                errors.Add(Error.CreateFrom("Card Expiry", "Card Expiry is Empty"));

            if (string.IsNullOrEmpty(card.Cvv))
                errors.Add(Error.CreateFrom("Card Cvv", "Card Cvv is Empty"));
 
            return errors.Count > 0 ? Result.Failed<Card>(errors) : Result.Ok<Card>(card);
        }

        public Result<RequestPaymentCommand> Validate(Card card, Guid merchantId, Money amount)
        {
            var validatedAmount =  ValidateAmount(amount);
            var validatedCard = ValidateCard(card);
            if (validatedCard.IsOk && validatedAmount.IsOk) 
                return Result.Ok<RequestPaymentCommand>();

            var errors = new List<Error>();
            errors.AddRange(validatedAmount.Errors);
            errors.AddRange(validatedCard.Errors);
            return Result.Failed<RequestPaymentCommand>(errors);
        }
    }
}
