using System;

namespace Calori.Domain.Models.CaloriAccount
{
    public class PersonalSlimmingPlan
    {
        public int Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public int? WeekNumber { get; set; }
        public decimal? Weight { get; set; }
        public int? CaloricNeeds { get; set; }
        public int? CurrentPlan { get; set; }
        public int? CurrentWeekDeficit { get; set; }
        public int? BurnedThisWeek { get; set; }
        public int? TotalBurned { get; set; }
    }
}