using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper; 
using Dapper.Contrib.Extensions;
using Domain;
using Domain.Payment.Events;
using Domain.Payment.Aggregate;
using Domain.Payment.Projection;

namespace Data
{
    public class PaymentProjectionRepository : IPaymentProjectionRepository
    {
        private readonly string _connectionString;

        public PaymentProjectionRepository(
            String connectionString
        )
        {
            _connectionString = connectionString;
        }

        public Result<PaymentProjection> Get(Guid id)
        {
            var sql = $"SELECT * FROM [PaymentProjections] WHERE PaymentId = '{id}';";
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var payments = connection.QueryFirstOrDefault<PaymentProjection>(sql);
                return Result.Ok(payments);
            }
            catch (Exception ex)
            {
                return Result.Failed<PaymentProjection>(Error.CreateFrom("PaymentProjection", ex));
            }
        }

        public Result<IEnumerable<PaymentProjection>> GetByMerchantId(Guid merchantId)
        {
            var sql = $"SELECT * FROM [PaymentProjections] WHERE MerchantId = '{merchantId}';";
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var payments = connection.Query<PaymentProjection>(sql);
                return Result.Ok(payments);
            }
            catch (Exception ex)
            {
                return Result.Failed<IEnumerable<PaymentProjection>>(Error.CreateFrom("PaymentProjection", ex));
            }
        }

        public Result<object> Add(PaymentProjection paymentProjection)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                    connection.Insert(paymentProjection);

                return Result.Ok<object>();
            }
            catch (Exception ex)
            {
                return Result.Failed<object>(Error.CreateFrom("Error when Adding to PaymentProjection", ex));
            }
        }
        private Result<object> Update(string sql)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand(sql, connection);
                    command.ExecuteNonQuery();
                    return Result.Ok<object>();
                }
            }
            catch (Exception ex)
            {
                return Result.Failed<object>(Error.CreateFrom("PaymentProjection", ex));
            }
        }

        public Result<object> Update(AcquiringBankPaymentProcessedEvent paymentEvent)
        {
            var sql =
                $"UPDATE [dbo].[PaymentProjections] " +
                $"SET [PaymentStatus] = '{paymentEvent.PaymentStatus.Id}', " +
                $"[LastUpdatedDate] = '{paymentEvent.TimeStamp}', " +
                $"[AcquiringBankId] = '{paymentEvent.AcquiringBankId}' " +
                $"WHERE PaymentId = '{paymentEvent.AggregateId}'";
            return Update(sql);
        }
         
        public Result<object> Update(AcquiringBankPaymentFailedEvent paymentEvent)
        {
            var sql =
                "UPDATE [dbo].[PaymentProjections] " +
                $"SET [PaymentStatus] = '{paymentEvent.PaymentStatus.Id}', " +
                $"[LastUpdatedDate] = '{paymentEvent.TimeStamp}', ";

            sql +=
                paymentEvent.AcquiringBankId.HasValue ? $"[AcquiringBankId] = '{paymentEvent.AcquiringBankId}', " : "";

            sql +=
                $"[FailedDetails] = '{paymentEvent.Details}' " +
                $"WHERE PaymentId = '{paymentEvent.AggregateId}'";

            return Update(sql);
        } 
    }
}
