using System.Linq;
using Domain.Payment.Aggregate;
using Domain.Payment.InputValidator;
using NUnit.Framework;

namespace Domain.Test
{
    class InputValidatorAmountValidatorTests
    {
        [Test]
        public void WHEN_Amount_has_no_error_THEN_return_Ok()
        {
            var amount = new Money(10 ,"Euro");

            var amountValidator = new AmountValidator();
            var actual = amountValidator.Validate(amount);

            Assert.True(actual.IsOk); 
        }

        [Test]
        public void WHEN_Amount_Currency_empty_THEN_return_Error()
        {
            var amount = new Money(10, "");

            var amountValidator = new AmountValidator();
            var actual = amountValidator.Validate(amount);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Amount", actual.Errors.First().Subject);
            Assert.AreEqual("Amount Currency is Empty", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_Amount_less_than_0_THEN_return_Error()
        {
            var amount = new Money(-1, "Euro");

            var amountValidator = new AmountValidator();
            var actual = amountValidator.Validate(amount);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Amount", actual.Errors.First().Subject);
            Assert.AreEqual("Amount should be more than 0", actual.Errors.First().Message);
        }
    }
}
