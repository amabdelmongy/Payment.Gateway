using System;
using System.Linq;
using Domain.Payment;
using Domain.Payment.Aggregate;
using Domain.Payment.Projection;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.v1
{
    [ApiController]
    [Route("api/v1/payment-details")]
    public class PaymentDetailsController : ControllerBase
    { 
        private readonly IPaymentProjectionRepository _paymentProjectionRepository;

        public PaymentDetailsController( 
            IPaymentProjectionRepository paymentProjectionRepository
        )
        {
            _paymentProjectionRepository = paymentProjectionRepository;
        }
         
        [HttpGet]
        [Route("{paymentId}")]
        public ActionResult Get(Guid paymentId)
        {
            var paymentResult =
                _paymentProjectionRepository.Get(paymentId);

            if (paymentResult.HasErrors)
                return new BadRequestObjectResult(
                    paymentResult.Errors
                        .Select(error => new
                        {
                            subject = error.Subject,
                            message = error.Message
                        }));

            if (paymentResult.Value == null)
                return new NotFoundResult();

            var payment = paymentResult.Value;

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

        [HttpGet]
        [Route("{merchantId}/get-by-merchant-id")]
        public ActionResult GetByMerchantId(Guid merchantId)
        {
            var paymentsResult =
                _paymentProjectionRepository.GetByMerchantId(merchantId);

            if (paymentsResult.HasErrors)
                return new BadRequestObjectResult(
                    paymentsResult.Errors
                        .Select(error => new
                        {
                            subject = error.Subject,
                            message = error.Message
                        }));

            if (paymentsResult.Value == null)
                return new NotFoundResult();

            var payments = paymentsResult.Value;

            return Ok(
                payments.Select(payment =>
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
                )
            );
        }
    }
}