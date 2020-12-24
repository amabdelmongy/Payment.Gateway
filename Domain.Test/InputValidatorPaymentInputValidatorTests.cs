using System.Linq;
using Domain.Payment.Aggregate;
using Domain.Payment.InputValidator;
using NUnit.Framework;

namespace Domain.Test
{
    class InputValidatorPaymentInputValidatorTests
    {
        [Test]
        public void WHEN_Card_and_Amount_has_no_error_THEN_return_Ok()
        {
            var card = new Card(
                PaymentStubsTests.CardTest.Number,
                PaymentStubsTests.CardTest.Expiry,
                PaymentStubsTests.CardTest.Cvv
            );
            var amount = new Money(10 ,"Euro");

            var paymentInputValidator = new PaymentInputValidator();
            var actual = 
                paymentInputValidator.Validate(
                    card,
                    amount
                );

            Assert.True(actual.IsOk); 
        }

        [Test]
        public void WHEN_card_has_error_THEN_return_Error()
        {
            var card = new Card(
                "",
                PaymentStubsTests.CardTest.Expiry,
                PaymentStubsTests.CardTest.Cvv
            );
            var amount = new Money(10, "Euro");

            var paymentInputValidator = new PaymentInputValidator();
            var actual =
                paymentInputValidator.Validate(
                    card,
                    amount
                );
             
            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Card Number", actual.Errors.First().Subject);
            Assert.AreEqual("Card Number is Empty", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_Amount_less_than_0_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubsTests.CardTest.Number,
                PaymentStubsTests.CardTest.Expiry,
                PaymentStubsTests.CardTest.Cvv
            );
            var amount = new Money(-1, "Euro");

            var paymentInputValidator = new PaymentInputValidator();
            var actual =
                paymentInputValidator.Validate(
                    card,
                    amount
                ); 

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Amount", actual.Errors.First().Subject);
            Assert.AreEqual("Amount should be more than 0", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN__Card_has_error_and_Amount_less_than_0_THEN_return_Errors()
        {
            var card = new Card(
                "",
                PaymentStubsTests.CardTest.Expiry,
                PaymentStubsTests.CardTest.Cvv
            );
            var amount = new Money(-1, "Euro");

            var paymentInputValidator = new PaymentInputValidator();
            var actual =
                paymentInputValidator.Validate(
                    card,
                    amount
                );

            Assert.True(actual.HasErrors);
            Assert.AreEqual(2, actual.Errors.Count());
            Assert.AreEqual("Invalid Amount", actual.Errors.First().Subject);
            Assert.AreEqual("Amount should be more than 0", actual.Errors.First().Message);
            Assert.AreEqual("Invalid Card Number", actual.Errors.Last().Subject);
            Assert.AreEqual("Card Number is Empty", actual.Errors.Last().Message);
        }
    }
}
