using System;
using Newtonsoft.Json;

namespace Domain.Payment.Events
{
    public abstract class Event
    {
        [JsonConstructor]
        protected Event(Guid aggregateId, DateTime timeStamp, int version, Type type)
        {
            AggregateId = aggregateId;
            TimeStamp = timeStamp;
            Version = version;
            Type = type.Name;
        }

        public Guid AggregateId { get; }
        public DateTime TimeStamp { get; }
        public int Version { get; }
        public string Type { get; }
    }
}
