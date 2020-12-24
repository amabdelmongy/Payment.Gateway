using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Domain.Payment;
using Domain.Payment.Aggregate;
using WebApi.dto;

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
    }
}