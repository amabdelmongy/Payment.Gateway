using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public abstract class Event
    { 
        protected Event(Guid aggregateId, DateTime timeStamp, int version)
        {
            AggregateId = aggregateId;
            TimeStamp = timeStamp;
            Version = version;
        }

        public Guid AggregateId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public int Version { get; private set; }
    }
}
