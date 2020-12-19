using System;
using System.Collections.Generic;

namespace Domain
{
    public interface IPaymentEventRepository
    {
        Result<IEnumerable<Event>> Get(Guid id);
        Result<IEnumerable<Event>> Get();
        Result<object> Add(Event paymentEvent);
    }
}
