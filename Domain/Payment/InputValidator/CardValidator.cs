using System.Collections.Generic;
using System.Text.RegularExpressions;
using Domain.Payment.Aggregate;

namespace Domain.Payment.InputValidator
{
    public class CardValidator
    {
        private const string InvalidCardNumber = "Invalid Card Number";
        private const string InvalidExpiry = "Invalid Expiry Date";
        private const string InvalidCvv ="Invalid CVV";

        public Result<object> Validate(Card card)
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
                    Error.CreateFrom( 
                        InvalidCardNumber,
                        "Card Number is Empty"
                    )
                );

            if (!Regex.Match(cardNumber, @"^\d{16}$").Success)
                return Result.Failed<object>(
                    Error.CreateFrom( 
                        InvalidCardNumber,
                        "Card Number is Invalid"
                    )
                );

            return Result.Ok<object>();
        }

        private Result<object> ValidateSecurityCode(string cvv)
        {
            if (string.IsNullOrEmpty(cvv))
                return Result.Failed<object>(
                    Error.CreateFrom( 
                        InvalidCvv,
                        "Card CVV is Empty")
                );

            if (!Regex.Match(cvv, @"^\d{3}$").Success)
                return Result.Failed<object>(
                    Error.CreateFrom( 
                        InvalidCvv,
                        "Card CVV is Invalid"
                    )
                );
            return Result.Ok<object>();
        }

        private Result<object> ValidateExpiry(string expiry)
        {
            if (string.IsNullOrEmpty(expiry))
                return Result.Failed<object>(Error.CreateFrom( InvalidExpiry, "Expire date is Empty")
                );

            if (!Regex.Match(expiry, @"^(0?[1-9]|1[012])/[0-9]{2}$").Success)
                return Result.Failed<object>(Error.CreateFrom( InvalidExpiry, "Expire date is Invalid")
                ); 
            return Result.Ok<object>();
        } 
    }
}