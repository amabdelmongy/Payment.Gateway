using System;
using Microsoft.AspNetCore.Mvc; 
using System.Collections.Generic;
using System.Linq;
using Service;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentController : ControllerBase
    { 

        public PaymentController()
        { 
        }

        [HttpPost]
        public void ProcessPayment( [FromBody] PaymentRequestDto paymentRequestDto)
        {
             
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
            public Guid Id { get; set; }

            public CardDto Card { get; set; }

            public Guid MerchantId { get; set; }

            public MoneyDto Amount { get; set; }
        }

        #endregion


    }
}
