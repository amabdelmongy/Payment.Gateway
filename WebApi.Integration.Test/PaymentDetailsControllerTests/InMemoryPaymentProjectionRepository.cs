using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Payment.Events;
using Domain.Payment.Projection;

namespace WebApi.Integration.Test.PaymentDetailsControllerTests
{
    public class InMemoryPaymentProjectionRepository : IPaymentProjectionRepository
    {
        readonly List<PaymentProjection> _paymentProjections = new List<PaymentProjection>();

        private Result<PaymentProjection> _resultGet;
        private Result<IEnumerable<PaymentProjection>> _resultGetByMerchantId;
        private Result<object> _resultObject;

        public InMemoryPaymentProjectionRepository WithNewGetResult (Result<PaymentProjection> resultGet)
        {
            _resultGet = resultGet;
            return this;
        }
        public InMemoryPaymentProjectionRepository WithNewGetByMerchantIdResult(Result<IEnumerable<PaymentProjection>> resultGetByMerchantId)
        {
            _resultGetByMerchantId = resultGetByMerchantId;
            return this;
        }

        public Result<IEnumerable<PaymentProjection>> GetByMerchantId(Guid merchantId)
        {
            if (_resultGetByMerchantId != null) return _resultGetByMerchantId;
            return Result.Ok<IEnumerable<PaymentProjection>>(
                _paymentProjections.
                    Where(t => 
                        t.MerchantId == merchantId
                    )
                    .ToList()
            );
        }

        public Result<object> Add(PaymentProjection paymentProjection)
        {
             _paymentProjections.Add(paymentProjection);
             return _resultObject;
        }

        public Result<object> Update(AcquiringBankPaymentProcessedEvent acquiringBankPaymentProcessedEvent)
        {
            return _resultObject;
        }

        public Result<object> Update(AcquiringBankPaymentFailedEvent acquiringBankPaymentFailedEvent)
        {
            return _resultObject;
        }

        Result<PaymentProjection> IPaymentProjectionRepository.Get(Guid id)
        {
            if (_resultGet != null) return _resultGet;
            return Result.Ok<PaymentProjection>(
                _paymentProjections
                    .FirstOrDefault(t =>
                    t.PaymentId == id
                    )
            );
        }
    }
}

