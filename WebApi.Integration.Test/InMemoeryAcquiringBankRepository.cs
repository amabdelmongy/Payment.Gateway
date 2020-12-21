using System; 
using Domain;
using Domain.AcquiringBank;
using Domain.Payment;

namespace WebApi.Integration.Test
{
    public class InMemoryAcquiringBankRepository : IAcquiringBankRepository
    {  
        private Result<Guid> _result;

        public InMemoryAcquiringBankRepository WithId(Guid id)
        {
            _result = Result.Ok<Guid>(id);
            return this;
        }

        public InMemoryAcquiringBankRepository WithNewResult(Result<Guid> result)
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
