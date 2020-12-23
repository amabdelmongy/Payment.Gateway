using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper; 
using Dapper.Contrib.Extensions;
using Domain;

namespace Data
{
    public class EventRepository : IEventRepository
    {
        private readonly string _connectionString;
        private readonly IDispatchRepository _paymentDispatchRepository;

        public EventRepository(
            string connectionString,
            IDispatchRepository paymentDispatchRepository
        )
        {
            _connectionString = connectionString;
            _paymentDispatchRepository = paymentDispatchRepository;
        }

        public Result<IEnumerable<Domain.Payment.Events.Event>> Get(Guid id)
        {
            var sql = $"SELECT * FROM [Events] WHERE AggregateId = '{id}';";
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var eventEntries = connection.Query<Event>(sql);
                    var events =
                        eventEntries
                            .Select(t =>
                                SerializeEvents.DeserializeEvent(t.Type, t.EventData)
                            )
                            .ToList();
                    return Result.Ok(events.AsEnumerable());
                }
            }
            catch (Exception ex)
            {
                return Result.Failed<IEnumerable<Domain.Payment.Events.Event>>(Error.CreateFrom("Get Payment Events", ex));
            }
        }

        public Result<object> AddEventToEventStore(Domain.Payment.Events.Event @event)
        {
            var eventDataResult = SerializeEvents.SerializeEvent(@event);
            if (!eventDataResult.IsOk)
                return Result.Failed<object>(eventDataResult.Errors);

            var insertModel = new Event
            {
                AggregateId = @event.AggregateId,
                CreatedOn = @event.TimeStamp,
                Version = @event.Version,
                EventData = eventDataResult.Value,
                Type = @event.Type
            };

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        connection.Insert(insertModel, transaction);

                        _paymentDispatchRepository
                            .AddEvent(
                                @event.Type,
                                eventDataResult.Value,
                                connection,
                                transaction
                            );
                        transaction.Commit();
                    }
                }

                return Result.Ok<object>();
            }
            catch (Exception ex)
            {
                return Result.Failed<object>(Error.CreateFrom("Error when Adding Event to Event table", ex));
            }
        }

        public Result<object> Add(Domain.Payment.Events.Event @event)
        {
            var result = AddEventToEventStore(@event);
            _paymentDispatchRepository.DeleteAndGetDispatchedEventsAsync();
            return result;
        }

        class Event
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
