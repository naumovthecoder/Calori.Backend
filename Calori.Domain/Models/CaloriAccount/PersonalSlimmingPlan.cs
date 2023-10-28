using System;
using Calori.Domain.Models.Enums;

namespace Calori.Domain.Models.CaloriAccount
{
    public class PersonalSlimmingPlan
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public int? WeekNumber { get; set; }
        public decimal? CurrentWeight { get; set; }
        public int? CaloricNeeds { get; set; }
        public int? Goal { get; set; }
        public int? CaloriSlimmingPlanId { get; set; }
        public CaloriSlimmingPlan CaloriSlimmingPlan { get; set; }
        public int? CurrentWeekDeficit { get; set; }
        public int? BurnedThisWeek { get; set; }
        public int? TotalBurned { get; set; }
        public int? WeeksToTarget { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}