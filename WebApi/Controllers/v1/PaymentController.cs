using System;
using System.Linq;
using Domain.Payment;
using Domain.Payment.Aggregate;
using Domain.Payment.Projection;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentWorkflow _paymentWorkflow;
        private readonly IPaymentService _paymentService; 

        public PaymentController(
            IPaymentWorkflow paymentWorkflow,
            IPaymentService paymentService
        )
        {
            _paymentWorkflow = paymentWorkflow;
            _paymentService = paymentService; 
        }
           
        [HttpPost]
        [Route("request-payment")]
        public ActionResult RequestPayment(
            [FromBody] PaymentRequestDto paymentRequestDto
        )
        {
            var paymentResult =
                _paymentWorkflow.Run(
                    new Card(
                        paymentRequestDto.Card.Number,
                        paymentRequestDto.Card.Expiry,
                        paymentRequestDto.Card.Cvv
                    ),
                    paymentRequestDto.MerchantId,
                    new Money(
                        paymentRequestDto.Amount.Value,
                        paymentRequestDto.Amount.Currency
                    )
                );

            if (paymentResult.HasErrors)
                return new BadRequestObjectResult(
                    paymentResult.Errors
                        .Select(error => new
                        {
                            subject = error.Subject,
                            message = error.Message
                        }));

            var paymentAggregate =
                _paymentService
                    .Get(paymentResult.Value.AggregateId)
                    .Value;

            return Ok(new
                {
                    AcquiringBankId = paymentAggregate.AcquiringBankId,
                    MerchantId = paymentAggregate.MerchantId,
                    PaymentId = paymentAggregate.PaymentId,
                }
            );
        }

        #region Input Dto 
        public class MoneyDto
        {
            public double Value { get; set; }
            public string Currency { get; set; }
        }

        public class CardDto
        {
            public string Number { get; set; }
            public string Expiry { get; set; }
            public string Cvv { get; set; }
        }

        public class PaymentRequestDto
        {
            public Guid MerchantId { get; set; }
            public MoneyDto Amount { get; set; }
            public CardDto Card { get; set; }
        } 
        #endregion
    }
}