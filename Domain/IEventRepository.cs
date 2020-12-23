using System;
using System.Collections.Generic;
using Domain.Payment.Events;

namespace Domain
{
    public interface IEventRepository
    {
        Result<IEnumerable<Event>> Get(Guid id);
        Result<object> Add(Event @event);
    }
}
