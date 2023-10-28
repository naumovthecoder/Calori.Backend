namespace Calori.Domain.Models.CaloriAccount
{
    public class CaloriSlimmingPlan
    {
        public int Id { get; set; }
        public int? Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Fats { get; set; }
        public decimal? Carbohydrates { get; set; }
    }
}