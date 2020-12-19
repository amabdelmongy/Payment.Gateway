using System;
using System.Collections.Generic;
using System.Text;
using Domain.Payment.Commands;

namespace Domain.Payment
{
    public interface IPaymentWorkflow
    {
        Result<Event> Run(Guid paymentId);
    }

    public class PaymentWorkflow : IPaymentWorkflow
    {
        private readonly IPaymentCommandHandler _paymentCommandHandler;

        public PaymentWorkflow(IPaymentCommandHandler paymentCommandHandler)
        {
            _paymentCommandHandler = paymentCommandHandler;
        }

        public Result<Event> Run(Guid paymentId)
        {
            var startProcessPaymentAtAcquiringBank = new StartProcessPaymentAtAcquiringBank(paymentId);  
            var processPaymentAtAcquiringBankStarted =
                _paymentCommandHandler.Handle(startProcessPaymentAtAcquiringBank);
            if (processPaymentAtAcquiringBankStarted.HasErrors)
                return Result.Failed<Event>(processPaymentAtAcquiringBankStarted.Errors);

            var processPaymentAtAcquiringBank = new ProcessPaymentAtAcquiringBank(paymentId);
            var result =
                _paymentCommandHandler.Handle(processPaymentAtAcquiringBank);
            if (result.HasErrors)
                return Result.Failed<Event>(result.Errors);


            return Result.Ok(processPaymentAtAcquiringBankStarted.Value);
        }
    }
}
