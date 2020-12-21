using System;
using System.Collections.Generic;
using System.Linq; 
using Domain;

namespace WebApi.Integration.Test
{
    public class InMemoryPaymentEventRepository : IPaymentEventRepository
    {
        List<Tuple<Guid, Event>> _tuples = new List<Tuple<Guid, Event>>();

        public Result<object> Add(Event paymentEvent)
        {
            _tuples.Add(new Tuple<Guid, Event>(paymentEvent.AggregateId, paymentEvent));
             return Result.Ok<object>();
        }

        public Result<IEnumerable<Event>> Get(Guid id)
        {
            var events =
                _tuples
                    .Where(t => t.Item1 == id)
                    .Select(t => t.Item2);

            return
                Result.Ok<IEnumerable<Event>>(events);

        }
    }
}

