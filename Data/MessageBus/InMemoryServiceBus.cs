using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.MessageBus;
using Domain.Payment.Events;

namespace Data
{ 
    public class InMemoryServiceBus : IServiceBusPublisher
    {
        private static readonly Dictionary<Type, List<Func<Event, Task>>> _dictionary = new Dictionary<Type, List<Func<Event, Task>>>();

        public void AddSubscription<T>(Func<Event, Task> handler)
        {

            if (_dictionary.TryGetValue(typeof(T), out var handlers))
            {
                handlers.Add(handler);
                _dictionary.TryAdd(typeof(T), handlers);

            }
            else
                _dictionary.Add(typeof(T), new List<Func<Event, Task>>() {handler});
        }

        public void Publish<T>(T @event) where T : Event
        {
            if (!_dictionary.TryGetValue(@event.GetType(), out var asyncHandlers))
            {
                return;
            }

            foreach (var handler in asyncHandlers)
            {
                var handlerFunction = handler;
                handlerFunction(@event);
            }
        }
    } 
}

