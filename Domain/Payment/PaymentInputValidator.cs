using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Payment.Commands;

namespace Domain.Payment
{
    public interface IPaymentInputValidator
    {
        Result<object> Validate(Card card, Money amount);
    }

    public class PaymentInputValidator : IPaymentInputValidator
    {
        private enum CardValidationErrorType
        {
            InvalidAmount,
            InvalidCardNumber,
            InvalidCvv,
            InvalidExpiry
        }

        public Result<object> Validate(
            Card card,
            Money amount
        )

        {
            var validatedAmountResult = ValidateAmount(amount);
            var validatedCardResult = ValidateCard(card);

            if (validatedCardResult.IsOk
                && validatedAmountResult.IsOk
            )
                return Result.Ok<object>();

            var errors = new List<Error>();
            errors.AddRange(validatedAmountResult.Errors);
            errors.AddRange(validatedCardResult.Errors);

            return Result.Failed<object>(errors);
        }

        private Result<object> ValidateAmount(Money amount)
        {
            var errors = new List<Error>();
            if (string.IsNullOrEmpty(amount.Currency))
                errors.Add(BuildError(CardValidationErrorType.InvalidAmount, "Amount Currency is Empty"));

            if (amount.Value <= 0)
                errors.Add(BuildError(CardValidationErrorType.InvalidAmount, "Amount should be more than 0"));

            return errors.Count > 0 ? Result.Failed<object>(errors) : Result.Ok<object>();
        }

        private Result<object> ValidateCard(Card card)
        {
            var errors = new List<Error>();
            errors.AddRange(ValidateCardNumber(card.Number).Errors);
            errors.AddRange(ValidateSecurityCode(card.Cvv).Errors);
            errors.AddRange(ValidateExpiry(card.Expiry).Errors);
            return errors.Count > 0 ? Result.Failed<object>(errors) : Result.Ok<object>();
        }

        private Result<object> ValidateCardNumber(string cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return Result.Failed<object>(
                    BuildError(CardValidationErrorType.InvalidCardNumber, "Card Number is Empty")
                );

            if (cardNumber.Length != 16)
                return Result.Failed<object>(
                    BuildError(CardValidationErrorType.InvalidCardNumber, "Card Number is Invalid")
                );

            return Result.Ok<object>();
        }

        private Result<object> ValidateSecurityCode(string cvv)
        {
            if (string.IsNullOrEmpty(cvv))
                return Result.Failed<object>(
                    BuildError(CardValidationErrorType.InvalidCvv, "Card CVV is Empty")
                );
             
            if (!Regex.Match(cvv, @"^\d{3}$").Success)
                return Result.Failed<object>(
                    BuildError(CardValidationErrorType.InvalidCvv, "Card CVV is Invalid")
                );
            return Result.Ok<object>();
        }

        private Result<object> ValidateExpiry(string expiry)
        {
            if (string.IsNullOrEmpty(expiry))
                return Result.Failed<object>(
                    BuildError(CardValidationErrorType.InvalidExpiry, "Expire date is Empty")
                );

            return Result.Ok<object>();
        }

        private static Error BuildError(CardValidationErrorType type, string message)
        {
            string subject = type switch
            {
                CardValidationErrorType.InvalidAmount => "Invalid Amount",
                CardValidationErrorType.InvalidCardNumber => "Invalid Card Number",
                CardValidationErrorType.InvalidCvv => "Invalid CVV",
                CardValidationErrorType.InvalidExpiry => "Invalid Expiry Date"
            };
            return Error.CreateFrom(subject, message);
        }
    }
}

