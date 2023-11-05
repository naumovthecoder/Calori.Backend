using Calori.Domain.Models.Payment;

namespace Calori.Domain.Models.CaloriAccount
{
    public class SubscriptionDetails
    {
        public int Id { get; set; }
        public string CaloriPriceId { get; set; }
        public CaloriPrice CaloriPrice { get; set; }
    }
}