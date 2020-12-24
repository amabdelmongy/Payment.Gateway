using System.Linq;
using Domain.Payment.Aggregate;
using Domain.Payment.InputValidator;
using NUnit.Framework;

namespace Domain.Test
{
    class InputValidatorCardValidatorTests
    {
        [Test]
        public void WHEN_card_has_no_error_THEN_return_Ok()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                PaymentStubs.CardTest.Expiry,
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.IsOk); 
        }

        [Test]
        public void WHEN_card_Number_empty_THEN_return_Error()
        {
            var card = new Card(
                "",
                PaymentStubs.CardTest.Expiry,
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Card Number", actual.Errors.First().Subject);
            Assert.AreEqual("Card Number is Empty", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_Number_has_char_THEN_return_Error()
        {
            var card = new Card(
                "asd",
                PaymentStubs.CardTest.Expiry,
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Card Number", actual.Errors.First().Subject);
            Assert.AreEqual("Card Number is Invalid", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_Number_has_less_than_16_length_THEN_return_Error()
        {
            var card = new Card(
                "1234567891234",
                PaymentStubs.CardTest.Expiry,
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Card Number", actual.Errors.First().Subject);
            Assert.AreEqual("Card Number is Invalid", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_Expiry_date_empty_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                "",
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Expiry Date", actual.Errors.First().Subject);
            Assert.AreEqual("Expire date is Empty", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_Expiry_has_char_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                "asas",
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Expiry Date", actual.Errors.First().Subject);
            Assert.AreEqual("Expire date is Invalid", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_Expiry_has_invalid_date_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                "121/25",
                PaymentStubs.CardTest.Cvv
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid Expiry Date", actual.Errors.First().Subject);
            Assert.AreEqual("Expire date is Invalid", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_cvv_empty_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                PaymentStubs.CardTest.Expiry,
                ""
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid CVV", actual.Errors.First().Subject);
            Assert.AreEqual("Card CVV is Empty", actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_card_cvv_has_char_THEN_return_Error()
        {
            var card = new Card(
                PaymentStubs.CardTest.Number,
                PaymentStubs.CardTest.Expiry,
                "asd"
            );

            var cardValidator = new CardValidator();
            var actual = cardValidator.Validate(card);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual("Invalid CVV", actual.Errors.First().Subject);
            Assert.AreEqual("Card CVV is Invalid", actual.Errors.First().Message);
        }
    }
}
