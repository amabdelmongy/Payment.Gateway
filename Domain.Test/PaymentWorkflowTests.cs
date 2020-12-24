using System.Linq;
using Domain.Payment;
using Domain.Payment.Aggregate;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;
using Domain.Payment.InputValidator;
using Moq;
using NUnit.Framework;

namespace Domain.Test
{
    public class PaymentWorkFlowTests
    {
        [Test]
        public void WHEN_PaymentInputValidator_has_error_THEN_return_Error()
        {
            var expectedError =
                Error.CreateFrom("subject", "message");

            var paymentCommandHandlerMock = new Mock<IPaymentCommandHandler>();
            paymentCommandHandlerMock
                .Setup(t =>
                    t.Handle(It.IsAny<PaymentCommand>())
                )
                .Returns(Result.Ok<Event>());

            var paymentInputValidatorMock = new Mock<IPaymentInputValidator>();
            paymentInputValidatorMock
                .Setup(t =>
                    t.Validate(It.IsAny<Card>(), It.IsAny<Money>())
                )
                .Returns(Result.Failed<object>(expectedError));

            var paymentWorkflow =
                new PaymentWorkflow(
                    paymentCommandHandlerMock.Object,
                    paymentInputValidatorMock.Object
                );

            var actual =
                paymentWorkflow.Run(
                    PaymentStubsTests.CardTest,
                    PaymentStubsTests.MerchantIdTest,
                    PaymentStubsTests.AmountTest
                );

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expectedError.Subject, actual.Errors.First().Subject);
            Assert.AreEqual(expectedError.Message, actual.Errors.First().Message);
        }

        [Test]
        public void WHEN_PaymentCommandHandler_has_error_THEN_return_Error()
        {
            var expectedError = Error.CreateFrom("subject", "message");

            var paymentCommandHandlerMock = new Mock<IPaymentCommandHandler>();
            paymentCommandHandlerMock
                .Setup(t =>
                    t.Handle(It.IsAny<PaymentCommand>())
                )
                .Returns(Result.Failed<Event>(expectedError));

            var paymentInputValidatorMock = new Mock<IPaymentInputValidator>();
            paymentInputValidatorMock
                .Setup(t =>
                    t.Validate(
                        It.IsAny<Card>(),
                        It.IsAny<Money>())
                )
                .Returns(Result.Ok<object>());

            var paymentWorkflow =
                new PaymentWorkflow(
                    paymentCommandHandlerMock.Object,
                    paymentInputValidatorMock.Object);

            var actual =
                paymentWorkflow.Run(
                    PaymentStubsTests.CardTest,
                    PaymentStubsTests.MerchantIdTest,
                    PaymentStubsTests.AmountTest);

            Assert.True(actual.HasErrors);
            Assert.AreEqual(1, actual.Errors.Count());
            Assert.AreEqual(expectedError.Subject, actual.Errors.First().Subject);
            Assert.AreEqual(expectedError.Message, actual.Errors.First().Message);
        }
    }
}