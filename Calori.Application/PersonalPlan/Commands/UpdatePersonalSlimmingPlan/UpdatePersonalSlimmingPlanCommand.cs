using System;
using Calori.Domain.Models.Enums;
using MediatR;

namespace Calori.Application.PersonalPlan.Commands.UpdatePersonalSlimmingPlan
{
    public class UpdatePersonalSlimmingPlanCommand : IRequest
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public int? WeekNumber { get; set; }
        public decimal? Weight { get; set; }
        public CaloriActivityLevel? CaloriActivityLevel { get; set; }
        public int? CaloricNeeds { get; set; }
        public int? Goal { get; set; }
        public int? CaloriSlimmingPlanId { get; set; }
        public int? CurrentWeekDeficit { get; set; }
        public int? BurnedThisWeek { get; set; }
        public int? TotalBurned { get; set; }
        public CaloriGender? Gender { get; set; }
        public int? CurrentCalori { get; set; }
    }
}