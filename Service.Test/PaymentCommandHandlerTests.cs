using System; 
using System.Linq;
using Domain;
using Domain.Payment;
using Domain.Payment.Commands;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Domain.Test
{
    public class PaymentCommandHandlerTests
    {
       
        //[Test]
        //public void GIVEN_PaymentCommandHandler_WHEN_Handle_RequestProcessPayment_THEN_Create_paymentRequestedEvent_Event_and_return_Ok()
        //{
        //    var paymentEventRepository = new Mock<IPaymentEventRepository>();
        //    paymentEventRepository
        //            .Setup(repository => repository.Add(It.IsAny<Event>()))
        //            .Returns(Result.Ok<object>());

        //    Payments payments = new Payments(paymentEventRepository.Object);

        //    var paymentCommandHandler = new PaymentCommandHandler(paymentEventRepository.Object , payments );

        //    var requestProcessPayment = new RequestProcessPayment(
        //    new Card(
        //        "1325464897",
        //        "24/04",
        //        "123"
        //    ),
        //    new Guid("377588c0-1d24-480d-aff6-a90f0c2fe536"),
        //    new Money(100.99, "Euro")
        //    );

        //    var actualResult = paymentCommandHandler.Handle(requestProcessPayment);
             
        //    Assert.IsTrue(actualResult.IsOk);
        //    var actual = actualResult.Value;
        //    Assert.AreEqual("PaymentRequestedEvent", actual.Type);
        //    Assert.AreEqual(requestProcessPayment.PaymentId, actual.AggregateId);
        //    Assert.AreEqual(1, actual.Version);
        //}
    }
}