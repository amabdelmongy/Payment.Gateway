using System;
using Microsoft.AspNetCore.Mvc; 
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Payment;
using Domain.Payment.Commands;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentCommandHandler _paymentCommandHandler;
        private readonly IRequestProcessPaymentInputValidator _requestProcessPaymentInputValidator;

        public PaymentController(IPaymentCommandHandler paymentCommandHandler, IRequestProcessPaymentInputValidator requestProcessPaymentInputValidator)
        {
            _paymentCommandHandler = paymentCommandHandler;
            _requestProcessPaymentInputValidator = requestProcessPaymentInputValidator;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new List<string>{"1","2","3"};
        }

        [HttpPost]
        [Route("request-process-payment")]
        public ActionResult RequestProcessPayment([FromBody] PaymentRequestDto paymentRequestDto)
        { 
            var requestProcessPayment = new RequestProcessPaymentFactory(_requestProcessPaymentInputValidator).From(
                new Card(
                    paymentRequestDto.Card.Number,
                    paymentRequestDto.Card.Expiry,
                    paymentRequestDto.Card.Cvv
                ),
                paymentRequestDto.MerchantId,
                new Money(paymentRequestDto.Amount.Value, paymentRequestDto.Amount.Currency)
            );

            if (requestProcessPayment.IsOk)
            {
               var result = _paymentCommandHandler.Handle(requestProcessPayment.Value);

                if (result.IsOk)
                    return Ok();

                return new BadRequestObjectResult(result.Errors);

            }
            else
            { 
                return new BadRequestObjectResult(
                    requestProcessPayment.Errors.Select(error => new
                {
                    subject = error.Subject,
                    message = error.Message
                }));
            }
        }
         

        #region Dto
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
