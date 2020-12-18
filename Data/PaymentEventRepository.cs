﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using Newtonsoft.Json;


using Dapper; 
using Dapper.Contrib.Extensions;

using Domain;
using Domain.Events;

namespace Data
{
     
    public class PaymentEventRepository : IPaymentEventRepository
    {
        private readonly string _connectionString;

        public PaymentEventRepository(String connectionString)
        {
            _connectionString = connectionString;
        } 

        public Result<IEnumerable<Event>> Get(Guid id)
        {
            var sql = $"SELECT * FROM PaymentEvent WHERE AggregateId = '{id}';";
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var eventEntries = connection.Query<PaymentEvent>(sql);
                    var events = eventEntries.Select(DeserializeEvent).ToList();
                    return Result.Ok(events.AsEnumerable());
                }
            }
            catch (Exception ex)
            {
                return Result.Failed<IEnumerable<Event>>(Error.CreateFrom("GetPaymentEvents", ex));
            }
        }

        public Result<IEnumerable<Event>> Get()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var eventEntries = connection.GetAll<PaymentEvent>();
                    var events = eventEntries.Select(DeserializeEvent).ToList();
                    return Result.Ok(events.AsEnumerable());
                }
            }
            catch (Exception ex)
            {
                return Result.Failed<IEnumerable<Event>>(Error.CreateFrom("GetAllPaymentEvents", ex));
            }
        }

        public Result<object> Add(Event @event)
        {
            var eventDataResult = SerializeEvent(@event);
            if (!eventDataResult.IsOk)
                return Result.Failed<object>(eventDataResult.Errors);

            var insertModel = new PaymentEvent
            {
                AggregateId = @event.AggregateId,
                CreatedOn = @event.TimeStamp,
                Version = @event.Version,
                EventData = eventDataResult.Value,
                Type = @event.Type
            };
            var result = Add(insertModel);
            return result;
        }

        private static Result<string> SerializeEvent(Event @event)
        {
            string eventData;
            switch (@event)
            {
                case PaymentProcessCreated paymentCreatedEvent:
                    eventData = JsonConvert.SerializeObject(paymentCreatedEvent);
                    break;
                default:
                    return Result.Failed<string>(
                        Error.CreateFrom("SerializePaymentEvent",  
                        $"Not valid event type"));
            }

            return Result.Ok(eventData);
        }

        private Result<object> Add(PaymentEvent paymentEvent)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Insert(paymentEvent);
                }
            }
            catch (Exception ex)
            {
                return Result.Failed<object>(Error.CreateFrom("PersistPaymentEvent", ex));
            }

            return Result.Ok<object>();
        }

        private static Event DeserializeEvent(PaymentEvent e)
        {
            return e.Type switch
            {
                nameof(PaymentProcessCreated)
                    => PaymentProcessCreated.CreateFrom(e.EventData),
                _ => throw new AggregateException($"Couldn't process the event of Type {e.Type}'")
            };
        }

        [Dapper.Contrib.Extensions.Table("PaymentEvents")]
        internal class PaymentEvent
        {
            public int Id { get; set; }
            public Guid AggregateId { get; set; }

            public DateTime CreatedOn { get; set; }
            public int Version { get; set; }
            public string Type { get; set; }
            public string EventData { get; set; }
        }
    }
}
