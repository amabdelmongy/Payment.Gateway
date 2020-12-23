using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Domain.Payment;
using Domain.Payment.Aggregate;
using Domain.Payment.Projection;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentWorkflow _paymentWorkflow;
        private readonly IPaymentService _paymentService;
        private readonly IPaymentProjectionRepository _paymentProjectionRepository;

        public PaymentController(
            IPaymentWorkflow paymentWorkflow,
            IPaymentService paymentService,
            IPaymentProjectionRepository paymentProjectionRepository
        )
        {
            _paymentWorkflow = paymentWorkflow;
            _paymentService = paymentService;
            _paymentProjectionRepository = paymentProjectionRepository;
        }
         
        [HttpGet]
        [Route("{paymentId}")]
        public ActionResult Get(Guid paymentId)
        {
            var payments =
                _paymentProjectionRepository.Get(paymentId);

            if (payments.HasErrors)
                return new BadRequestObjectResult(
                    payments.Errors
                        .Select(error => new
                        {
                            subject = error.Subject,
                            message = error.Message
                        }));

            if (!payments.Value.Any())
                return new NotFoundResult();

            var payment = payments.Value.First();

            return Ok(
                new
                {
                    paymentId = payment.PaymentId,
                    MerchantId = payment.MerchantId,
                    CardNumber = payment.CardNumber,
                    CardExpiry = payment.CardExpiry,
                    CardCvv = payment.CardCvv,
                    Amount = payment.Amount,
                    Currency = payment.Currency,
                    PaymentStatus = payment.PaymentStatus,
                    LastUpdatedDate = payment.LastUpdatedDate,
                    AcquiringBankId = payment.AcquiringBankId
                }
            );
        }

        [HttpPost]
        [Route("request-process-payment")]
        public ActionResult RequestProcessPayment(
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