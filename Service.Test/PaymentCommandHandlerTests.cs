using System; 
using System.Linq;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;
using Domain.Payment.CommandHandlers;
using Domain.Payment.Commands;
using Domain.Payment.Events;
using Moq;
using NUnit.Framework;

namespace Domain.Test
{
    public class PaymentCommandHandlerTests
    {
        private Mock<IPayments> paymentsMock()
        { 
            var paymentsMock = new Mock<IPayments>();

            paymentsMock
                .Setup(payments =>
                    payments.Get(It.IsAny<Guid>()))
                .Returns(Result.Ok<PaymentAggregate>(PaymentStubs.PaymentAggregateTest()));
            return paymentsMock;
        }
        private Mock<IRequestProcessPaymentCommandHandler> requestProcessPaymentCommandHandlerMock()
        {
            var requestProcessPaymentCommandHandler = new Mock<IRequestProcessPaymentCommandHandler>();
            requestProcessPaymentCommandHandler
                .Setup(repository =>
                    repository.Handle(It.IsAny<RequestPaymentCommand>())
                )
                .Returns(It.IsAny<Result<Event>>());
            return requestProcessPaymentCommandHandler;
        }

        private Mock<IFailAcquiringBankPaymentCommandHandler> failAcquiringBankPaymentCommandHandlerMock()
        {
            var requestProcessPaymentCommandHandlerMock = new Mock<IFailAcquiringBankPaymentCommandHandler>();
            requestProcessPaymentCommandHandlerMock
                .Setup(repository =>
                    repository.Handle(
                        It.IsAny<FailAcquiringBankPaymentCommand>(),
                        It.IsAny<int>())
                )
                .Returns(It.IsAny<Result<Event>>());
            return requestProcessPaymentCommandHandlerMock;
        }

        private Mock<IProcessAcquiringBankPaymentCommandHandler> processAcquiringBankPaymentCommandHandlerMock()
        {
            var processAcquiringBankPaymentCommandHandlerMock = new Mock<IProcessAcquiringBankPaymentCommandHandler>();
            processAcquiringBankPaymentCommandHandlerMock
                .Setup(handler =>
                    handler.Handle(
                        It.IsAny<PaymentAggregate>(),
                        It.IsAny<ProcessAcquiringBankPaymentCommand>())
                )
                .Returns(It.IsAny<Result<Event>>());
            return processAcquiringBankPaymentCommandHandlerMock;
        }

        [Test]
        public void
            GIVEN_PaymentCommandHandler_WHEN_Handle_RequestPaymentCommand_THEN_should_call_requestProcessPaymentCommandHandler()
        {  
            var requestProcessPaymentCommandHandler = requestProcessPaymentCommandHandlerMock();

            var paymentCommandHandler =
                new PaymentCommandHandler(
                    paymentsMock().Object,
                    requestProcessPaymentCommandHandler.Object,
                    processAcquiringBankPaymentCommandHandlerMock().Object,
                    failAcquiringBankPaymentCommandHandlerMock().Object
                );

            var requestPaymentCommand = new RequestPaymentCommand(
                PaymentStubs.CardTest,
                PaymentStubs.MerchantIdTest,
                PaymentStubs.AmountTest
            );

            paymentCommandHandler.Handle(requestPaymentCommand);

            requestProcessPaymentCommandHandler.Verify(
                mock => mock.Handle(It.IsAny<RequestPaymentCommand>()),
                Times.Once());
        } 

        [Test]
        public void
            GIVEN_PaymentCommandHandler_WHEN_Handle_ProcessAcquiringBankPaymentCommand_THEN_should_call_ProcessAcquiringBankPaymentCommandHandler()
        {  
            var processAcquiringBankPaymentCommandHandlerMock = this.processAcquiringBankPaymentCommandHandlerMock();

            var paymentCommandHandler =
                new PaymentCommandHandler(
                    paymentsMock().Object,
                    requestProcessPaymentCommandHandlerMock().Object,
                    processAcquiringBankPaymentCommandHandlerMock.Object,
                    failAcquiringBankPaymentCommandHandlerMock().Object
                );

            var processAcquiringBankPaymentCommand =
                new ProcessAcquiringBankPaymentCommand(
                    PaymentStubs.PaymentIdTest
                );

            paymentCommandHandler.Handle(processAcquiringBankPaymentCommand);

            processAcquiringBankPaymentCommandHandlerMock
                .Verify(mock =>
                        mock.Handle(
                            It.IsAny<PaymentAggregate>(),
                            It.IsAny<ProcessAcquiringBankPaymentCommand>()),
                    Times.Once());
        }

        [Test]
        public void
            GIVEN_PaymentCommandHandler_WHEN_Handle_FailAcquiringBankPaymentCommand_THEN_should_call_FailAcquiringBankPaymentCommandHandler()
        { 
            var processAcquiringBankPaymentCommandHandlerMock = this.processAcquiringBankPaymentCommandHandlerMock();

            var paymentCommandHandler =
                new PaymentCommandHandler(
                    paymentsMock().Object,
                    requestProcessPaymentCommandHandlerMock().Object,
                    processAcquiringBankPaymentCommandHandlerMock.Object,
                    failAcquiringBankPaymentCommandHandlerMock().Object
                );

            var requestPaymentCommand =
                new ProcessAcquiringBankPaymentCommand(
                    PaymentStubs.PaymentIdTest
                );

            paymentCommandHandler.Handle(requestPaymentCommand);

            processAcquiringBankPaymentCommandHandlerMock
                .Verify(mock =>
                        mock.Handle(
                            It.IsAny<PaymentAggregate>(),
                            It.IsAny<ProcessAcquiringBankPaymentCommand>()),
                    Times.Once());
        }

        [Test]              
        public void GIVEN_PaymentCommandHandler_and_payments_return_error_WHEN_Handle_FailAcquiringBankPaymentCommand_THEN_Create_PaymentRequestedEvent_Event_and_return_Ok()
        {
            var expectedError =
                Error.CreateFrom(
                    "Error Subject",
                    "Error Message");

            var paymentsMock = new Mock<IPayments>();
            paymentsMock
                .Setup(repository =>
                    repository.Get(It.IsAny<Guid>()))
                .Returns(Result.Failed<PaymentAggregate>(expectedError));

            var paymentCommandHandler =
                new PaymentCommandHandler(
                    paymentsMock.Object,
                    requestProcessPaymentCommandHandlerMock().Object,
                    processAcquiringBankPaymentCommandHandlerMock().Object,
                    failAcquiringBankPaymentCommandHandlerMock().Object
                );

            var processAcquiringBankPaymentCommand =
                new ProcessAcquiringBankPaymentCommand(
                    PaymentStubs.PaymentIdTest
                );

            var actualResult = 
                paymentCommandHandler.Handle(processAcquiringBankPaymentCommand);

            Assert.IsTrue(actualResult.HasErrors);
            Assert.AreEqual(1, actualResult.Errors.Count());
            var error = actualResult.Errors.First();

            Assert.AreEqual(expectedError.Subject, error.Subject);
            Assert.AreEqual(expectedError.Message, error.Message);
        }
    }
}