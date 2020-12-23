using System.Collections.Generic;
using Domain.Payment.Aggregate;

namespace Domain.Payment.InputValidator
{
    internal class AmountValidator
    { 
        private const string ErrorTitleInvalidAmount = "Invalid Amount";
        public Result<object> Validate(Money amount)
        {
            var errors = new List<Error>();
            if (string.IsNullOrEmpty(amount.Currency))
                errors.Add(Error.CreateFrom(ErrorTitleInvalidAmount, "Amount Currency is Empty"));

            if (amount.Value <= 0)
                errors.Add(Error.CreateFrom(ErrorTitleInvalidAmount, "Amount should be more than 0"));

            return errors.Count > 0 ? Result.Failed<object>(errors) : Result.Ok<object>();
        }
    }
}