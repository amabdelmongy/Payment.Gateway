using System;
using Newtonsoft.Json;

namespace Domain
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

        public Guid AggregateId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public int Version { get; private set; }
        public string Type { get; set; }
    }
}
