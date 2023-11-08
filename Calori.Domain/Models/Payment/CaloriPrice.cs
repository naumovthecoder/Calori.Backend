using Calori.Domain.Models.CaloriAccount;

namespace Calori.Domain.Models.Payment
{
    public class CaloriPrice
    {
        // priceId: "price_1O7hQCHNRk8vDVhuTqLeBqO6",
        // name: "Every 2 weeks",
        // price: "100",

        public string PriceId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string DailyPrice { get; set; }
        public int CaloriSlimmingPlanId { get; set; }
        public CaloriSlimmingPlan CaloriSlimmingPlan { get; set; }
    }
}