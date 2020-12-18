using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Events
{
    public class PaymentProcessGatewayRequested : Event
    {
        public PaymentProcessGatewayRequested(Guid aggregateId, DateTime timeStamp, int version, Type type) : base(aggregateId, timeStamp, version, type)
        {
        }
    }
}
