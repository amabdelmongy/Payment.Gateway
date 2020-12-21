
namespace Domain.Payment
{
    public class PaymentStatus
    {
        public static readonly PaymentStatus ProcessStarted =
            new PaymentStatus("processing");

        public static readonly PaymentStatus Processed =
            new PaymentStatus("processed");

        public static readonly PaymentStatus Failed =
            new PaymentStatus("failed");

        private PaymentStatus(string id)
        {
            Id = id;
        }
        public string Id { get; }
    }
}
