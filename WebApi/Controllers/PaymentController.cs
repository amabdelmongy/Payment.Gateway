using System;
using Microsoft.AspNetCore.Mvc; 
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Commands;
using Service;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentCommandHandler _paymentCommandHandler;

        public PaymentController(IPaymentCommandHandler paymentCommandHandler)
        {
            _paymentCommandHandler = paymentCommandHandler;
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
            var requestProcessPayment = new RequestProcessPaymentFactory().From(
                new Card(
                    paymentRequestDto.Card.Number,
                    paymentRequestDto.Card.Expiry,
                    paymentRequestDto.Card.Cvv
                ),
                paymentRequestDto.MerchantId,
                new Money(Double.Parse(paymentRequestDto.Amount.Value), paymentRequestDto.Amount.Currency)
            );
            var result = _paymentCommandHandler.Handle(requestProcessPayment.Value);

            return Ok();
        }

        #region Dto
        public class MoneyDto
        {  
            public string Value { get; set; }
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
