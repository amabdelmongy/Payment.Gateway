using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.MessageBus;
using Domain.Payment.Events;

namespace Data.MessageBus
{ 
    public class InMemoryServiceBus : IServiceBusPublisher
    {
        private static readonly Dictionary<Type, List<Func<Event, Task>>> Dictionary 
            = new Dictionary<Type, List<Func<Event, Task>>>();

        public void AddSubscription<T>(Func<Event, Task> handler)
        {

            if (Dictionary.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Add(handler);
                Dictionary.TryAdd(typeof(T), handlers);
            }
            else
                Dictionary.Add(typeof(T), new List<Func<Event, Task>>() {handler});
        }

        public void Publish<T>(T @event) where T : Event
        {
            if (!Dictionary.TryGetValue(@event.GetType(), out var asyncHandlers))
                return;

            foreach (var handler in asyncHandlers)
            {
                var handlerFunction = handler;
                handlerFunction(@event);
            }
        }
    } 
}

