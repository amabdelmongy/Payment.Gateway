using System;

namespace Domain.AcquiringBank
{
    public class RejectedAcquiringBankError : Error
    {
        private RejectedAcquiringBankError(
            Guid acquiringBankResultId,
            string subject,
            string message) : base(subject, null, message)
        {
            AcquiringBankResultId = acquiringBankResultId;
        }

        public Guid AcquiringBankResultId { get; }

        public static Error CreateFrom(Guid acquiringBankResultId, string subject, string message = null)
        {
            return new RejectedAcquiringBankError(acquiringBankResultId, subject, message);
        }
    }
}
