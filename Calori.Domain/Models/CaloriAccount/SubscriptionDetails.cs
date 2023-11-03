namespace Calori.Domain.Models.CaloriAccount
{
    public class SubscriptionDetails
    {
        public int Id { get; set; }
        public int? PaymentPeriodId { get; set; }
        public PaymentPeriod PaymentPeriod { get; set; }
    }
}