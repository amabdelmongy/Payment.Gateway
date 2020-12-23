using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Domain.MessageBus;

namespace Data
{
    public interface IDispatchRepository
    {
        void AddEvent(
            string eventType,
            string eventData,
            SqlConnection connection,
            IDbTransaction transaction
        );

        Task DeleteAndGetDispatchedEventsAsync();
    }

    public class DispatchRepository : IDispatchRepository
    {
        private readonly string _connectionString;
        private readonly IServiceBusPublisher _serviceBusPublishers;

        public DispatchRepository(IServiceBusPublisher serviceBusPublishers, string connectionString)
        {
            _serviceBusPublishers = serviceBusPublishers;
            _connectionString = connectionString;
        } 

        public void AddEvent(
            string eventType,
            string eventData,
            SqlConnection connection,
            IDbTransaction transaction
        )
        {
            var insertModel = new DispatchRepository.Dispatch
            {
                CreatedOn = DateTime.Now,
                EventData = eventData,
                Type = eventType
            };
            connection.Insert(insertModel, transaction);
        }

        public async Task DeleteAndGetDispatchedEventsAsync()
        {
            var sql = $"DELETE FROM [dbo].[Dispatchs] OUTPUT DELETED.[Type], DELETED.[EventData]";
           
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Transaction = transaction;
                        var dataReader = command.ExecuteReader();
                        while (dataReader.Read())
                        {
                            var @event = SerializeEvents.DeserializeEvent(
                                dataReader[0].ToString(),
                                dataReader[1].ToString()
                            );

                            _serviceBusPublishers.Publish(@event);
                        } 
                        dataReader.Dispose();
                    } 
                    transaction.Commit();
                }
            }
        }

        class Dispatch
        {
            public int Id { get; set; }
            public string EventData { get; set; }
            public string Type { get; set; }
            public DateTime CreatedOn { get; set; }
        }
    }
}
