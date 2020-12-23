using System;
using Domain;
using Domain.AcquiringBank;
using Domain.Payment.Aggregate;

namespace WebApi.Integration.Test.PaymentControllerTests
{
    public class InMemoryAcquiringBankFacade : IAcquiringBankFacade
    {  
        private Result<Guid> _result;

        public InMemoryAcquiringBankFacade WithId(Guid id)
        {
            _result = Result.Ok<Guid>(id);
            return this;
        }

        public InMemoryAcquiringBankFacade WithNewResult(Result<Guid> result)
        {
            _result = result;
            return this;
        }

        public Result<Guid> ProcessPayment(PaymentAggregate paymentAggregate)
        {
            return _result;
        }
    }
}
