using System;
using AutoMapper;
using Calori.Application.Common.Mappings;
using Calori.Domain.Models.CaloriAccount;
using Calori.Domain.Models.Enums;

namespace Calori.Application.PersonalPlan.Queries
{
    public class PersonalPlanDetailsVm : IMapWith<PersonalSlimmingPlan>
    {
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
        public DateTime? PausedAt { get; set; }

        public SubscriptionStatus? SubscriptionStatus { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PersonalSlimmingPlan, PersonalPlanDetailsVm>();
        }
    }
}