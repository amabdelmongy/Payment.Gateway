namespace Domain.Payment.Aggregate
{
    public class PaymentStatus
    {
        public static readonly PaymentStatus ProcessStarted =
            new PaymentStatus("processing");

        public static readonly PaymentStatus Processed =
            new PaymentStatus("processed");

        public static readonly PaymentStatus Failed =
            new PaymentStatus("failed");

        public PaymentStatus(string id)
        {
            Id = id;
        }
        public string Id { get; }
    }

 
}
